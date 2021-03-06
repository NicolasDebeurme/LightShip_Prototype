using Niantic.ARDK;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.VirtualStudio.VpsCoverage;
using Niantic.ARDK.VPSCoverage;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;

public class LocationService : MonoBehaviour
{
    //ILocation-----------------------------
    public ILocationService _iLocationService;
    public TextMeshProUGUI distanceText;

    private LatLng _yourPosition;
    private LatLng _pointToReach;
    private float _anglePlayerToNorth;
    private float _anglePlayerTarget;

    private Image _compassArrow;
    private readonly float _minimumDistanceReachPoint = 10;

    //WebView-------------------------------
    //private string Url = "test.html";
    //private WebViewObject webViewObject;
    //NOT USED FOR THE MOMENT ------------ TO IMPLEMENT

    private void Start()
    {

        _iLocationService = LocationServiceFactory.Create(GameManager.Instance.runtimeEnv);

        PlayUIupdate();

    }

    public void Init(TextMeshProUGUI distanceText, Image compassArrow)
    {
        this.distanceText = distanceText;
        _compassArrow = compassArrow;
    }
    private void OnStatusUpdated(LocationStatusUpdatedArgs args)
    {
        //Debug.Log("LocationService :" + args.Status.ToString());
    }

    //private void OnCompassUpdated(CompassUpdatedArgs args)
    //{
    //    UpdateMapCompass(args.TrueHeading);
    //}
    //private void OnLocationUpdated(LocationUpdatedArgs args)
    //{
    //    if(!_isSpoofEnabled)
    //    {
    //        UpdateMapCoord(args.LocationInfo.Coordinates, new LatLng(args.LocationInfo.Coordinates.Latitude+ 1, args.LocationInfo.Coordinates.Longitude + 1));
    //    }
    //    else
    //    {
    //        UpdateMapCoord(_spoofLocation, new LatLng(_spoofLocation.Latitude + 1, _spoofLocation.Longitude + 1));
    //    }
    //}

    //    IEnumerator Start()
    //    {

    //        webViewObject = new GameObject("WebViewObject").AddComponent<WebViewObject>();
    //        webViewObject.Init(
    //            cb: (msg) =>
    //            {
    //                Debug.Log(string.Format("CallFromJS[{0}]", msg));
    //            },
    //            started: (msg) =>
    //            {
    //                Debug.Log(string.Format("CallOnStarted[{0}]", msg));
    //            },
    //            hooked: (msg) =>
    //            {
    //                Debug.Log(string.Format("CallOnHooked[{0}]", msg));
    //            },
    //            ld: (msg) =>
    //            {
    //                Debug.Log(string.Format("CallOnLoaded[{0}]", msg));
    //#if UNITY_EDITOR_OSX || (!UNITY_ANDROID && !UNITY_WEBPLAYER && !UNITY_WEBGL)
    //                // NOTE: depending on the situation, you might prefer
    //                // the 'iframe' approach.
    //                // cf. https://github.com/gree/unity-webview/issues/189
    //#if true
    //                webViewObject.EvaluateJS(@"
    //                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
    //                    window.Unity = {
    //                      call: function(msg) {
    //                        window.webkit.messageHandlers.unityControl.postMessage(msg);
    //                      }
    //                    }
    //                  } else {
    //                    window.Unity = {
    //                      call: function(msg) {
    //                        window.location = 'unity:' + msg;
    //                      }
    //                    }
    //                  }
    //                ");
    //#else
    //                webViewObject.EvaluateJS(@"
    //                  if (window && window.webkit && window.webkit.messageHandlers && window.webkit.messageHandlers.unityControl) {
    //                    window.Unity = {
    //                      call: function(msg) {
    //                        window.webkit.messageHandlers.unityControl.postMessage(msg);
    //                      }
    //                    }
    //                  } else {
    //                    window.Unity = {
    //                      call: function(msg) {
    //                        var iframe = document.createElement('IFRAME');
    //                        iframe.setAttribute('src', 'unity:' + msg);
    //                        document.documentElement.appendChild(iframe);
    //                        iframe.parentNode.removeChild(iframe);
    //                        iframe = null;
    //                      }
    //                    }
    //                  }
    //                ");
    //#endif
    //#elif UNITY_WEBPLAYER || UNITY_WEBGL
    //                webViewObject.EvaluateJS(
    //                    "window.Unity = {" +
    //                    "   call:function(msg) {" +
    //                    "       parent.unityWebView.sendMessage('WebViewObject', msg)" +
    //                    "   }" +
    //                    "};");
    //#endif
    //                webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
    //            }
    //            //transparent: false,
    //            //zoom: true,
    //            //ua: "custom user agent string",
    //            //// android
    //            //androidForceDarkMode: 0,  // 0: follow system setting, 1: force dark off, 2: force dark on
    //            //// ios
    //            //enableWKWebView: true,
    //            //wkContentMode: 0,  // 0: recommended, 1: mobile, 2: desktop
    //            //wkAllowsLinkPreview: true,
    //            //// editor
    //            //separated: false
    //            );
    //#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
    //        webViewObject.bitmapRefreshCycle = 1;
    //#endif
    //        // cf. https://github.com/gree/unity-webview/pull/512
    //        // Added alertDialogEnabled flag to enable/disable alert/confirm/prompt dialogs. by KojiNakamaru ? Pull Request #512 ? gree/unity-webview
    //        //webViewObject.SetAlertDialogEnabled(false);

    //        // cf. https://github.com/gree/unity-webview/pull/728
    //        //webViewObject.SetCameraAccess(true);
    //        //webViewObject.SetMicrophoneAccess(true);

    //        // cf. https://github.com/gree/unity-webview/pull/550
    //        // introduced SetURLPattern(..., hookPattern). by KojiNakamaru ? Pull Request #550 ? gree/unity-webview
    //        //webViewObject.SetURLPattern("", "^https://.*youtube.com", "^https://.*google.com");

    //        // cf. https://github.com/gree/unity-webview/pull/570
    //        // Add BASIC authentication feature (Android and iOS with WKWebView only) by takeh1k0 ? Pull Request #570 ? gree/unity-webview
    //        //webViewObject.SetBasicAuthInfo("id", "password");

    //        //webViewObject.SetScrollbarsVisibility(true);

    //        webViewObject.SetMargins(5, 5, 5, Screen.height/6);
    //        webViewObject.SetTextZoom(100);  // android only. cf. https://stackoverflow.com/questions/21647641/android-webview-set-font-size-system-default/47017410#47017410
    //        webViewObject.SetVisibility(true);

    //#if !UNITY_WEBPLAYER && !UNITY_WEBGL
    //        if (Url.StartsWith("http"))
    //        {
    //            webViewObject.LoadURL(Url.Replace(" ", "%20"));
    //        }
    //        else
    //        {
    //            var exts = new string[]{
    //                ".html"  // should be last
    //            };
    //            foreach (var ext in exts)
    //            {
    //                var url = Url.Replace(".html", ext);
    //                var src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
    //                var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
    //                byte[] result = null;
    //                if (src.Contains("://"))
    //                {  // for Android
    //#if UNITY_2018_4_OR_NEWER
    //                    // NOTE: a more complete code that utilizes UnityWebRequest can be found in https://github.com/gree/unity-webview/commit/2a07e82f760a8495aa3a77a23453f384869caba7#diff-4379160fa4c2a287f414c07eb10ee36d
    //                    var unityWebRequest = UnityWebRequest.Get(src);
    //                    yield return unityWebRequest.SendWebRequest();
    //                    result = unityWebRequest.downloadHandler.data;
    //#else
    //                    var www = new WWW(src);
    //                    yield return www;
    //                    result = www.bytes;
    //#endif
    //                }
    //                else
    //                {
    //                    result = System.IO.File.ReadAllBytes(src);
    //                }
    //                System.IO.File.WriteAllBytes(dst, result);
    //                if (ext == ".html")
    //                {
    //                    webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
    //                    break;
    //                }
    //            }
    //        }
    //#else
    //        if (Url.StartsWith("http")) {
    //            webViewObject.LoadURL(Url.Replace(" ", "%20"));
    //        } else {
    //            webViewObject.LoadURL("StreamingAssets/" + Url.Replace(" ", "%20"));
    //        }
    //#endif
    //        yield break;
    //    }

    //private void UpdateMapCoord(LatLng pos, LatLng dest)
    //{
    //    Debug.Log("Map updated");
    //    webViewObject.EvaluateJS(@"updateMapCoord(["
    //        + pos.Longitude.ToString().Replace(',','.') + "," 
    //        + pos.Latitude.ToString().Replace(',', '.') + "],[" 
    //        + dest.Longitude.ToString().Replace(',', '.') + "," 
    //        + dest.Latitude.ToString().Replace(',', '.') 
    //        + "]);");
    //}

    //private void UpdateMapCompass(float trueHeading)
    //{
    //    webViewObject.EvaluateJS(@"updateMapCompass("+trueHeading.ToString().Replace(',', '.') + ");");
    //}

    internal void Destroy()
    {
        Destroy(this);
        GameStateSystem.LocationService = null;
    }
    internal void PauseUIupdate()
    {
        _iLocationService.Stop();
        _iLocationService.LocationUpdated -= OnLocationUpdated;
        _iLocationService.StatusUpdated -= OnStatusUpdated;
        _iLocationService.CompassUpdated -= OnCompassUpdated;
    }
    internal void PlayUIupdate()
    {
        _pointToReach = Enums.Places_Coord[GameManager.Instance._actualGameState.ActualNode.data.place];

        if (_pointToReach != null)
        {
            _iLocationService.Start(0.001f, 0.001f);
            _iLocationService.LocationUpdated += OnLocationUpdated;
            _iLocationService.StatusUpdated += OnStatusUpdated;
            _iLocationService.CompassUpdated += OnCompassUpdated;
        }
        else
            throw new Exception("ERROR: Place doesn't exist or isn't initialized");
    }

    //Gps
    private void OnCompassUpdated(CompassUpdatedArgs args)
    {
        _anglePlayerToNorth = args.TrueHeading;
        UpdateCompass();
    }
    private void OnLocationUpdated(LocationUpdatedArgs args)
    {

        if (_iLocationService.Status == Niantic.ARDK.LocationService.LocationServiceStatus.Running)
        {
#if UNITY_EDITOR
            _yourPosition = new LatLng(50.8554550137514f, 3.634270943007247f);
#else
            _yourPosition = args.LocationInfo.Coordinates;
#endif
        }

        AsReachPosition();
    }

    private void AsReachPosition()
    {
        distanceText.text = _yourPosition.Distance(_pointToReach).ToString("N2") + " meters";
        if (_yourPosition.Distance(_pointToReach) < _minimumDistanceReachPoint && GameStateSystem.Instance != null)
        {
            GameManager.Instance._actualGameState.GetState().NextState();
        }
    }

    private void UpdateCompass()
    {
        Vector2 v1 = new((float)(_pointToReach.Longitude - _yourPosition.Longitude), (float)(_pointToReach.Latitude - _yourPosition.Latitude));
        Vector2 v2 = new(0, 1);


        _anglePlayerTarget = -Vector2.SignedAngle(v1, v2);

        _compassArrow.gameObject.transform.rotation = Quaternion.Euler(0, 0, (_anglePlayerToNorth + _anglePlayerTarget));
        //Debug.Log(_anglePlayerToNorth + " " + _anglePlayerTarget);
    }
}
