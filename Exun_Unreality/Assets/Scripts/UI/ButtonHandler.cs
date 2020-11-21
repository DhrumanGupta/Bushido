using System;
using System.Collections;
using Game.Networking;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

namespace Game.UI
{
    public class ButtonHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField ipInput = null;
        private string ip;

        [SerializeField] private TMP_InputField portInput = null;
        private int port;
        
        [SerializeField] private TextMeshProUGUI errorText = null;
        
        [SerializeField] private TextMeshProUGUI tutText = null;

        private bool isIpSet = false;
        
        public void SetIpPort()
        {
            string[] ipValues = ipInput.text.Split('.');
            if (ipValues.Length != 4)
            {
                StartCoroutine(ShowError($"An IP can only have 4 set of values! {ipValues.Length} is not an option!"));
                return;
            }
            
            foreach (string ipValue in ipValues)
            {
                if (string.IsNullOrWhiteSpace(ipValue) || ipValue.Length > 3)
                {
                    StartCoroutine(ShowError("An IP can only have 3 digits per value!"));
                    return;
                }
                
                try
                {
                    int ipInt = int.Parse(ipValue);
                    if (ipInt > 255 || ipInt < 0)
                    {
                        StartCoroutine(ShowError("IP values range further than 256. Make sure the IP is valid!"));
                        return;
                    }
                }
                catch (Exception e)
                {
                    print(e);
                    StartCoroutine(ShowError("An IP can only contain dots and integers!"));
                }
            }

            ip = ipInput.text;
            port = int.Parse(portInput.text);

            if (port < 1024 || port > 49151)
            {
                StartCoroutine(ShowError("The port number can only range from 1024 - 49151"));
                return;
            }

            Client.Instance.ip = ip;
            Client.Instance.port = port;
            
            print("Ip Set!");
            isIpSet = true;
        }

        public void ConnectBtn()
        {
            if (!isIpSet) return;
            transform.GetChild(0).gameObject.SetActive(false);
            Client.Instance.ConnectToServer();
            StartCoroutine(ShowTutuorial());
        }
        
        private IEnumerator ShowError(string msg)
        {
            errorText.SetText(msg);
            yield return new WaitForSeconds(2f);
            errorText.SetText("");
        }

        private IEnumerator ShowTutuorial()
        {
            tutText.gameObject.SetActive(true);
            tutText.SetText("Press WASD to move");
            yield return new WaitForSeconds(3f);
            
            tutText.SetText("Click to shoot");
            yield return new WaitForSeconds(3f);
            
            tutText.SetText("Press E to revive your teammate if defeated");
            yield return new WaitForSeconds(3f);
            
            tutText.SetText("");
            tutText.gameObject.SetActive(false);
        }
    }
}
