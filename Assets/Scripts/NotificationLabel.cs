using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class NotificationLabel : MonoBehaviour
{
    Text _label;

    void Awake()
    {
        _label = GetComponent<Text>();
        Core.SubscribeEvent("OnNotificationUpdate", OnNotificationUpdate);
    }

    void OnDestroy()
    {
        Core.UnsubscribeEvent("OnNotificationUpdate", OnNotificationUpdate);
    }

    // Use this for initialization
    void Start ()
    {
	}

    object OnNotificationUpdate(object sender, object args)
    {
        if (args is string)
        {
            _label.text = (string)args;
        }

        return null;
    }
}
