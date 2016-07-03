using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Volley
{
    [RequireComponent(typeof(Text))]
    public class StatusLabel : MonoBehaviour
    {
        [SerializeField] float ClearDelay;

        Text _label;
        Animator _anim;

        void Awake()
        {
            _anim = GetComponent<Animator>();
            _label = GetComponent<Text>();

            if (_label != null)
            {
                _label.text = "";
            }

            Core.SubscribeEvent("OnStatusUpdate", OnStatusUpdate);
        }

        void OnDestroy()
        {
            Core.UnsubscribeEvent("OnStatusUpdate", OnStatusUpdate);

        }

        object OnStatusUpdate(object sender, object args)
        {
            if (args is string)
            {
                string status = (string)args;
                _label.text = status;
                _anim.SetTrigger("show");
                //StartCoroutine(ShowText(status));
                //Invoke("ClearText", ClearDelay);
            }

            return null;
        }

        void ClearText()
        {
            _label.text = string.Empty;
        }
    }
}
