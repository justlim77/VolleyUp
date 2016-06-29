using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Volley
{
    [RequireComponent(typeof(Text))]
    public class ComboLabel : MonoBehaviour
    {
        Text _label;

	    void Awake ()
        {
            _label = GetComponent<Text>();
            if (_label != null)
            {
                _label.text = "No Combo";
            }

            Core.SubscribeEvent("OnComboUpdate", OnComboUpdate);
	    }

        void Start() { }

        object OnComboUpdate(object sender, object args)
        {
            if (args is int)
            {
                int combo = (int)args;
                string msg = "No combo";
                if (combo > 1 && combo < GameManager.Instance.comboMax)
                {
                    msg = string.Format("COMBO {0}x", combo.ToString());
                }
                else if (combo >= GameManager.Instance.comboMax)
                {
                    msg = "COMBO MAX";
                }

                _label.text = msg;
            } 

            return null;
        }
    }
}
