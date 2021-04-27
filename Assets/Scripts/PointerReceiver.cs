using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LCM;
using LCM.LCM;
#pragma warning disable 0649

public class PlayerPointer{

    public float timeSinceLastUpdate = 0;
    public string color;
    public string name;
    public string id;
    public GameObject sprite;
    public Vector3 position;

    public PlayerPointer(string _id, string _name, Vector3 _position, string _color){
        id = _id;
        name = _name;
        position = _position;
        color = _color;
    }

    public void UpdatePosition (Vector3 _position){
        position = _position;
        timeSinceLastUpdate = 0;
    }
    public void UpdateColor (string _color){
        color = _color;
    }
}
public class PointerReceiver : MonoBehaviour
{
    [SerializeField]
    GameObject prefab;
    [SerializeField]
    MousePointer mousePointerScript;
    private static PointerReceiver instance;
    public static Dictionary<string, PlayerPointer> playerDictionary =
    new Dictionary<string, PlayerPointer>();
    LCM.LCM.LCM myLCM;

    // Start is called before the first frame update
    void Start()
    { 
        instance = this;

        myLCM = new LCM.LCM.LCM();

        myLCM.SubscribeAll(new SimpleSubscriber());

    }

    // Update is called once per frame
    void Update()
    {   
        foreach( KeyValuePair<string, PlayerPointer> kvp in playerDictionary )
        {
            if (kvp.Value.sprite == null){
                GameObject sprite = Instantiate(prefab, kvp.Value.position, Quaternion.identity);
                SpriteRenderer spriterenderer = sprite.GetComponent<SpriteRenderer>();
                spriterenderer.color = mousePointerScript.colorDictionary[kvp.Value.color];
                kvp.Value.sprite = sprite;
            }
            else{
                if (kvp.Value.timeSinceLastUpdate > 1f)
                {
                    kvp.Value.sprite.SetActive(false);
                }
                else{
                    kvp.Value.sprite.SetActive(true);

                }
                Transform transform = kvp.Value.sprite.GetComponent<Transform>();
                SpriteRenderer spriterenderer = kvp.Value.sprite.GetComponent<SpriteRenderer>();
                spriterenderer.color = mousePointerScript.colorDictionary[kvp.Value.color];
                transform.position = kvp.Value.position;
            }
            kvp.Value.timeSinceLastUpdate += Time.deltaTime;
        }
    }

    public static void ProcessMessage(string id, string name, Vector3 position, string color){
        if (playerDictionary.ContainsKey(id)){
            playerDictionary[id].UpdatePosition(position);
            if (playerDictionary[id].color != color){
                playerDictionary[id].UpdateColor(color);
            }
        }
        else{
            playerDictionary.Add(id, new PlayerPointer(id, name, position, color) );
        }
    }

   class SimpleSubscriber : LCM.LCM.LCMSubscriber
    {
        public void MessageReceived(LCM.LCM.LCM lcm, string channel, LCM.LCM.LCMDataInputStream dins)
        {
            if (channel == "PLAYER_POINTERS")
            {
                Debug.Log("HII");
                mwt.rovvr_multiplayer_pointer_t msg = new mwt.rovvr_multiplayer_pointer_t(dins);
                Vector3 position = new Vector3((float)msg.position_x, (float)msg.position_y, (float)msg.position_z);
                ProcessMessage(msg.id, msg.name, position, msg.color);
            }
            else
            {
                // Debug.Log("Unknown Channel: " + channel);
            }
        }
    }
}
