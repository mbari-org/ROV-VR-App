using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#pragma warning disable 0649

public class MousePointer : MonoBehaviour
{
    // [SerializeField]
    // Transform laserTransform;
    // [SerializeField]
    // GameObject laser;
    // [SerializeField]
    // GameObject broadcastMessage;
    // [SerializeField]
    // Renderer laserRenderer;
    private bool active;
    private Plane plane;
    private Ray ray;
    float distance;
    private string playerID;
    private string playerName = "";
    private string playerColor = "red";

    // LCM
    LCM.LCM.LCM myLCM;
    Vector3 worldPosition;

    // Laser Color
    public Dictionary<string, Color> colorDictionary =
    new Dictionary<string, Color>();
    string[] colorList;
    int colorIndex;

    void Awake(){
        // Create projection plane for raycasting
        plane = new Plane(Vector3.back, 400);
        // GenerateUniqueID();
        GenerateColorDictionary();
        // SetLaserColor(playerColor);
    }
    // Start is called before the first frame update
    void Start()
    {
        //Initialize LCM 
        myLCM = new LCM.LCM.LCM();

    }

    // Update is called once per frame
    void Update()
    {   
        // if (Input.GetKeyDown(KeyCode.C))
        // {
        //     colorIndex = (colorIndex + 1) % colorList.Length;
        //     SetLaserColor(colorList[colorIndex]);
        // }

        // if (Input.GetMouseButtonDown(0)){
        //     active = true;
        //     laser.SetActive(true);
        //     broadcastMessage.SetActive(true);

        // }
        // if (Input.GetMouseButtonUp(0)){
        //     active = false;   
        //     laser.SetActive(false);
        //     broadcastMessage.SetActive(false);

        // }
        // if (!active){
        //     return;
        // }
        // ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        // if (plane.Raycast(ray, out distance))
        // {
        //     worldPosition = ray.GetPoint(distance);
        // }

        // laserTransform.LookAt(worldPosition);
        // PublishPointerData(worldPosition);

    }

    void GenerateUniqueID(){
        playerID = System.Guid.NewGuid().ToString();;
    }
    void GenerateColorDictionary(){
        colorDictionary.Add("red", Color.red);
        colorDictionary.Add("magenta", Color.magenta);
        colorDictionary.Add("yellow", Color.yellow);
        colorDictionary.Add("green", Color.green);  
        colorDictionary.Add("blue", Color.blue);


        colorList = new string[]{
            "blue",
            "red",
            "magenta",
            "yellow",
            "green"
        };
        playerColor = colorList[colorIndex];
    }

    // void SetLaserColor(string color){
    //     playerColor = color;
    //     laserRenderer.material.SetColor("_Color", colorDictionary[color]);
    // }


    void PublishPointerData(Vector3 position){
        mwt.rovvr_multiplayer_pointer_t msg = new mwt.rovvr_multiplayer_pointer_t();

        msg.id = playerID;
        msg.name = playerName;
        msg.color = playerColor;
        msg.position_x = position.x;
        msg.position_y = position.y;
        msg.position_z = position.z;
        myLCM.Publish("PLAYER_POINTERS", msg);
    }

}
