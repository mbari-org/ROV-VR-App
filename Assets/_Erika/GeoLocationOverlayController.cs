using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GeoLocationOverlayController : MonoBehaviour
{
    // Listener
    public GameObject listenerGameObject;
    private LCMListener listener;

    // GameObject References
    public GameObject rovLonTextGameObject;
    public GameObject rovLatTextGameObject;
    public GameObject shipLonTextGameObject;
    public GameObject shipLatTextGameObject;
    public GameObject rovArrowGameObject;
    public GameObject boatShiftGameObject;
    public GameObject dotGameObject;

    // Tranforms
    public Transform boatShiftTransform;


    // UI Components
    private TextMeshProUGUI rovLonText;
    private TextMeshProUGUI rovLatText;
    private TextMeshProUGUI shipLatText;
    private TextMeshProUGUI shipLonText;

    // Constants
    double R = 6378.137 * 1000; // Radius of earth in M

    // Coordinates
    private double[] originalLatLong;
    private bool gotOrigin;
    private double scaleFactor = 10;


    // Start is called before the first frame update
    void Start()
    {
        listener = listenerGameObject.GetComponent<LCMListener>();

        rovLonText = rovLonTextGameObject.GetComponent<TextMeshProUGUI>();
        rovLatText = rovLatTextGameObject.GetComponent<TextMeshProUGUI>();
        shipLonText = shipLonTextGameObject.GetComponent<TextMeshProUGUI>();
        shipLatText = shipLatTextGameObject.GetComponent<TextMeshProUGUI>();

        boatShiftTransform = boatShiftGameObject.GetComponent<Transform>();

        gotOrigin = false;
    }

    // Update is called once per frame
    void Update()
    {   // Set Origin
        if (!gotOrigin){
            var ROVLat = listener.ROVLat;
            var ROVLon = listener.ROVLon;
            if (ROVLat != 0 && ROVLon != 0){
                originalLatLong = new double[] {ROVLat, ROVLon};
                gotOrigin = true;
                StartCoroutine(UpdateGUI());
            }
            else{
                return;
            }
        }
        
        // Update Text
        rovLonText.SetText(listener.ROVLon.ToString());
        rovLatText.SetText(listener.ROVLat.ToString());
        shipLonText.SetText(listener.ShipLon.ToString());
        shipLatText.SetText(listener.ShipLat.ToString());

        // Update Rotation
        rovArrowGameObject.transform.rotation = Quaternion.Euler(0f, 0f, (float) listener.Yaw);
    }

    double[] CalculateMeters(double lat, double lon){
        var dLat = R * lat * Mathf.PI / 180 - R * originalLatLong[0] * Mathf.PI / 180;
        var dLon = R * lon * Mathf.PI / 180 - R * originalLatLong[1] * Mathf.PI / 180;
        return new double[] {dLat, dLon}; // meters
    }

    IEnumerator UpdateGUI() 
    {
        while (true) {
            double[] lonLat = CalculateMeters(listener.ROVLat, listener.ROVLon);
            Instantiate(
                dotGameObject, 
                new Vector3( (float) (lonLat[0] * scaleFactor), (float) (lonLat[1] * scaleFactor), 0), 
                Quaternion.identity, 
                boatShiftGameObject);
            yield return new WaitForSeconds(1f);
        }
        
    }
}
