using System;
using System.Collections.Generic;
using System.Text;
using Flux.Data;
using Flux.Event;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using EventHandler = Flux.Event.EventHandler;

namespace Chrome.Retro
{
    public class RetScoreHUD : MonoBehaviour
    {
        [FoldoutGroup("Dependencies"), SerializeField] private EventHandler handler;
        [FoldoutGroup("Dependencies"), SerializeField] private TMP_Text title;
        [FoldoutGroup("Dependencies"), SerializeField] private TMP_Text scoreboard;
        [FoldoutGroup("Dependencies"), SerializeField] private CanvasGroup scoreGroup;
        [FoldoutGroup("Dependencies"), SerializeField] private TMP_InputField usernameField;
        [FoldoutGroup("Dependencies"), SerializeField] private CanvasGroup replayGroup;

        [FoldoutGroup("Values"), SerializeField] private bool wipe;
        
        private int insertion;
        private float elapsedTime;
        private List<(string name, float time)> board = new List<(string name, float time)>();
        
        void Awake()
        { 
            if (wipe) PlayerPrefs.DeleteAll();

            for (var i = 1; i < 6; i++)
            {
                if (PlayerPrefs.HasKey($"{i}-Name")) board.Add((PlayerPrefs.GetString($"{i}-Name"), PlayerPrefs.GetFloat($"{i}-Time")));
                else board.Add(("NNN", -1.0f));
            }

            WriteToScoreboard();
            handler.AddDependency(Events.Subscribe(RetEvent.OnGameWon, OnGameWon));
        }

        private void WriteToScoreboard()
        {
            var content = new StringBuilder();
            for (var i = 0; i < board.Count; i++)
            {
                var time = board[i].time < 0 ? "XX:XX" : board[i].time.ConvertToTime();
                content.AppendLine($"[{i}] {board[i].name} - {time}");
            }

            scoreboard.text = content.ToString();
        }
        private void SaveScoreboard()
        {
            for (var i = 0; i < board.Count; i++)
            {
                PlayerPrefs.SetString($"{i}-Name", board[i].name);
                PlayerPrefs.SetFloat($"{i}-Score", board[i].time);
            }
        }
        
        void OnGameWon()
        {
            var timer = Repository.Get<RetTimerHUD>(RetReference.Timer);

            insertion = -1;
            elapsedTime = timer.ElapsedTime;
            title.text = $"YOU WON <size=50%>in {elapsedTime.ConvertToTime()}</size>";
            
            for (var i = 0; i < board.Count; i++)
            {
                if (board[i].time > 0.0f && elapsedTime >= board[i].time) continue;

                insertion = i;
                break;
            }

            if (insertion == -1)
            {
                scoreGroup.Toggle(false);
                replayGroup.Toggle(true);
            }
            else
            {
                scoreGroup.Toggle(true);
                replayGroup.Toggle(false);

                RetGameControl.isLocked = true;
                
                usernameField.text = string.Empty;
                usernameField.onSubmit.AddListener(OnUsernameSubmitted);
                
                /*usernameField.ActivateInputField();
                usernameField.Select();
                usernameField.OnPointerClick (new PointerEventData(EventSystem.current));
                EventSystem.current.SetSelectedGameObject(usernameField.gameObject, new BaseEventData(EventSystem.current));*/
                Events.Subscribe(RetEvent.OnScreenDisplay, OnScreenDisplay);
            }
        }

        void OnScreenDisplay()
        {
            Events.Unsubscribe(RetEvent.OnScreenDisplay, OnScreenDisplay);
            
            usernameField.ActivateInputField();
            usernameField.Select();
        }

        void OnUsernameSubmitted(string username)
        {
            usernameField.onSubmit.RemoveListener(OnUsernameSubmitted);

            if (insertion == -1) return;
            
            scoreGroup.Toggle(false);
            replayGroup.Toggle(true);
            
            board.Insert(insertion, (username.ToUpper(), elapsedTime));
            board.RemoveAt(board.Count - 1);
            
            SaveScoreboard();
            WriteToScoreboard();

            RetGameControl.isLocked = false;
        }
    }
}