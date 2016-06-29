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
                _label.text = "";
            }

            Core.SubscribeEvent("OnComboUpdate", OnComboUpdate);
	    }

        object OnComboUpdate(object sender, object args)
        {
            if (args is int)
            {
                int combo = (int)args;
                _label.text = string.Format("COMBO x{0}", combo.ToString());
            } 

            return null;
        }
    }
}
