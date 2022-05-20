using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections;

public class PlayerController : MonoBehaviour
{

    protected Transform _XForm_Camera;
    protected Transform _XForm_Parent;
    Rigidbody _XForm_RB;
    Transform XForm_Parent;
    protected Vector3 _LocalRotation;
    protected float _CameraDistance = 1000f;
    Vector3 vec;
    public Transform pointToRotate;
    public float MoveSens;
    public float MouseSensitivity = 56.5f;
    public float ScrollSensitvity = 2f;
    public float OrbitDampening = 10f;
    public float ScrollDampening = 6f;

    public bool CameraDisabled = false;

    public GameObject infoCanvas;

    GameObject sat;

    private bool moving = false;
    private bool inFocusMode = false;

    public Text activeText;
    public Info activeInfo = null;

    private Vector3 last;

    // Use this for initialization
    public void Start()
    {
        // temporary placeholder so unity doesnt complain
        sat = gameObject;
        _CameraDistance = 2000f;
        this._XForm_Camera = this.transform;
        this._XForm_Parent = this.transform.parent;
        this._XForm_RB = _XForm_Camera.gameObject.GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (inFocusMode && activeInfo != null)
        {
            activeText.text = activeInfo.date;         
        }
    }

    public void LateUpdate()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            CameraDisabled = !CameraDisabled;
        }

        if (!CameraDisabled)
        {
            if (Input.GetMouseButton(2))
            {
                Cursor.lockState = CursorLockMode.Locked;
                // Rotation of the Camera based on Mouse Coordinates
                if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
                {
                    _LocalRotation.x += Input.GetAxis("Mouse X") * MouseSensitivity;
                    _LocalRotation.y += Input.GetAxis("Mouse Y") * MouseSensitivity;

                    //Clamp the y Rotation to horizon and not flipping over at the top
                    if (_LocalRotation.y < -75f)
                        _LocalRotation.y = -75f;
                    else if (_LocalRotation.y > 75f)
                        _LocalRotation.y = 75f;

                }


            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }

            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float ScrollAmount = Input.GetAxis("Mouse ScrollWheel") * ScrollSensitvity;

                ScrollAmount *= (this._CameraDistance * 0.3f);

                this._CameraDistance += ScrollAmount * -1f;

                this._CameraDistance = Mathf.Clamp(this._CameraDistance, 1.5f, 1000000f);
            }
        }

        //Actual Camera Rig Transformations
        if (!inFocusMode)
        {
            Quaternion QT = Quaternion.Euler(_LocalRotation.y, _LocalRotation.x, 0);
            this._XForm_Parent.rotation = Quaternion.Lerp(this._XForm_Parent.rotation, QT, Time.deltaTime * OrbitDampening);

            if (this._XForm_Camera.localPosition.z != this._CameraDistance * -1f)
            {
                this._XForm_Camera.localPosition = new Vector3(0f, 0f, Mathf.Lerp(this._XForm_Camera.localPosition.z, this._CameraDistance * -1f, Time.deltaTime * ScrollDampening));
            }





            if (Input.GetKey(KeyCode.W))
            {
                _XForm_Parent.Translate(0, 0, MoveSens * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.S))
            {
                _XForm_Parent.Translate(0, 0, -MoveSens * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.A))
            {
                _XForm_Parent.Translate(-MoveSens * Time.deltaTime, 0, 0);
            }

            if (Input.GetKey(KeyCode.D))
            {
                _XForm_Parent.Translate(MoveSens * Time.deltaTime, 0, 0);
            }

            vec.x = _XForm_Parent.position.x;
            vec.y = 0;
            vec.z = _XForm_Parent.position.z;
            _XForm_Parent.position = vec;

        }

        if (Input.GetMouseButtonDown(0) && !moving)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                sat = hit.transform.gameObject;

                moving = true;
                inFocusMode = true;


            }
        }


        if (moving)
        {
            if (10000f * Time.deltaTime < Vector3.Distance(transform.position, sat.transform.position))
                transform.position = Vector3.MoveTowards(transform.position, sat.transform.position, 10000f * Time.deltaTime);
            else
                transform.position = Vector3.MoveTowards(transform.position, sat.transform.position, Vector3.Distance(transform.position, sat.transform.position) - 50f);

            if (Vector3.Distance(transform.position, sat.transform.position) < 100f)
            {
                transform.LookAt(sat.transform.position);
                moving = false;

                infoCanvas.SetActive(true);

                Transform[] transforms = infoCanvas.GetComponentsInChildren<Transform>();

                foreach (Transform t in transforms)
                {
                    if (t.gameObject.name == "Name")
                    {
                        string name = sat.GetComponent<Info>().GetName();
                        t.GetComponent<Text>().text = name;
                    }

                    if (t.gameObject.name == "NORAD")
                    {
                        int NORAD_ID = sat.GetComponent<Info>().NORAD_ID;
                        t.GetComponent<Text>().text = "NORAD ID: " + NORAD_ID.ToString();
                    }

                    if (t.gameObject.name == "Altitude")
                    {
                        int alt = (int)sat.GetComponent<Info>().latlongalt[2];
                        t.GetComponent<Text>().text = "Altitude: " + alt.ToString() + " km";
                    }
                    if (t.gameObject.name == "LaunchDate")
                    {
                        Info info = sat.GetComponent<Info>();
                        Thread thread = new Thread(new ThreadStart(info.SetDate));
                        thread.Start();
                        activeInfo = sat.GetComponent<Info>();
                        activeText = t.GetComponent<Text>();
                    }

                }
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (inFocusMode)
            {
                inFocusMode = false;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                infoCanvas.SetActive(false);

               
            }
                

            if (moving)
                moving = false;
        }
    }
}