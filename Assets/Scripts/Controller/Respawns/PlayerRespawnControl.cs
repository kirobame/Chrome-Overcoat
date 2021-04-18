using System.Collections;
using Flux.Data;
using UnityEngine;

namespace Chrome
{
    public class PlayerRespawnControl : RespawnControl
    {
        protected override IEnumerator Routine()
        {
            if (index < 0 || !Repository.TryGet<CanvasGroup>(Interface.Death, out var panel)) yield break;

            var halfDuration = duration * 0.5f;
            var time = 0.0f;
            
            while (time < halfDuration)
            {
                time += Time.deltaTime;
                panel.alpha = time / halfDuration;
                
                yield return new WaitForEndOfFrame();
            }
            panel.alpha = 1.0f;
            
            yield return new WaitForSeconds(halfDuration);
            
            if (!TryGetRespawnAnchor(out var anchor)) yield break;

            transform.position = anchor.position;
            gameObject.SetActive(true);

            var goal = 0.25f;
            time = goal;
            
            while (time > 0)
            {
                time -= Time.deltaTime;
                panel.alpha = time / goal;
                
                yield return new WaitForEndOfFrame();
            }
            panel.alpha = 0.0f;
        }
    }
}