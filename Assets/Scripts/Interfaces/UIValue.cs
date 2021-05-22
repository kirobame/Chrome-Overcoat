using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Flux.Data;
using Flux;

namespace Chrome
{
    [CreateAssetMenu(fileName = "NewUIValue", menuName = "Chrome Overcoat/UIValue")]

    public class UIValue : ScriptableObject
    {
        Dictionary<string, object> values = new Dictionary<string, object>();
        //public float value { get; private set; }

        [SerializeField] List<DynamicFlag> HUDAdresses = new List<DynamicFlag>();

        /*
        bool isBootedup = false;
        void Bootup()
        {
            foreach (var hud in HUDAdresses)
                hud.Bootup();
            isBootedup = true;
        }
        */

        public void Set(object newValue, string tag)
        {
            /*
            if (!isBootedup)
                Bootup();
            */

            if (!values.ContainsKey(tag))
                values.Add(tag, newValue);
            else
                values[tag] = newValue;

            foreach (var hud in HUDAdresses)
            {
                hud.Bootup();
                IDeprecatedHUD _hud = Repository.Get<IDeprecatedHUD>(hud.Value);
                _hud.Refresh(newValue, tag);
            }
        }




        /*
        [SerializeField] DynamicFlag hud;
        public void bipboop()
        {
            hud.Bootup();
            IHUD _hud = Repository.Get<IHUD>(hud.Value);
        }*/
    }
}
