using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class Brain : Agent
{
    Rigidbody rBody;
    Transform Car;
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
        Car = GetComponent<Transform>();

    }
    public Transform FrontRightWheel;
    public Transform FrontLeftWheel;
    public Transform BackRightWheel;
    public Transform BackLeftWheel;
    public float trialTime = 10;

    private float horizontalInput;
    private float verticalInput;
    private float currentSteerAngle;
    private float currentBrakeForce;
    private bool isBreaking = false;
    private bool inRoad;
    private int direction = 0;
    private int inRoadCounter=0;
    private float elapsed=0;
    private float reward=0;
    private float distance = 0;

    public GameObject spawnPoint;

    [SerializeField] private float motorForce = 50f;
    [SerializeField] private float brakeForce = 0f;
    [SerializeField] private float maxSteeringAngle = 30f;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;

    [Header("Sensors Transform")]
    [SerializeField] private Transform sensorRoot;
    [SerializeField] private Transform front;
    [SerializeField] private Transform frontRight;
    [SerializeField] private Transform frontLeft;
    [SerializeField] private Transform frontRightest;
    [SerializeField] private Transform frontLeftest;
    [SerializeField] private Transform right;
    [SerializeField] private Transform left;
    [SerializeField] private Transform back;
    [SerializeField] private Transform backRight;
    [SerializeField] private Transform backLeft;
    [SerializeField] private Transform backRightest;
    [SerializeField] private Transform backLeftest;
    [SerializeField] private LayerMask raycastIgnore;

    [Header("Raycasts")]
    [SerializeField] private RaycastHit hitDirFront;
    [SerializeField] private RaycastHit hitDirFrontRight;
    [SerializeField] private RaycastHit hitDirFrontRightest;
    [SerializeField] private RaycastHit hitDirFrontLeft;
    [SerializeField] private RaycastHit hitDirFrontLeftest;
    [SerializeField] private RaycastHit hitDirRight;
    [SerializeField] private RaycastHit hitDirRightLeft;
    [SerializeField] private RaycastHit hitDirBack;
    [SerializeField] private RaycastHit hitDirBackRight;
    [SerializeField] private RaycastHit hitDirBackLeft;
    [SerializeField] private RaycastHit hitDirBackLeftest;
    [SerializeField] private RaycastHit hitDirBackRightest;

    [Header("Sensors")]
    public float sensorLength = 2f;
    public Vector3 frontSensorStartPos = new Vector3(0f, 0.2f, 0f);
    public float frontSideSensorStartPos = 0.2f;
    public float frontSensorAngle = 30;

    GUIStyle guiStyle = new GUIStyle();
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup (new Rect (10, 10, 250, 150));
        GUI.Box (new Rect (0,0,140,140), "Stats", guiStyle);
        GUI.Label(new Rect (10,25,200,30), string.Format("Time: {0:0.00}",elapsed), guiStyle);
        GUI.Label(new Rect (10,50,200,30), "Reward: "+reward, guiStyle);
        GUI.Label(new Rect (10,75,200,30), "inRoad: "+inRoad, guiStyle);
        GUI.Label(new Rect (10,100,200,30), "Direction: "+direction, guiStyle);
        GUI.EndGroup ();
    }


    public override void OnEpisodeBegin()
    {
        Debug.Log("Iniciando treinamento...");
        this.transform.position = spawnPoint.transform.position;
        // If the Agent fell, zero its momentum
        if (this.transform.localPosition.y <= 0.04960653)
        {
            this.rBody.angularVelocity = Vector3.zero;
            inRoadCounter = 0;
            reward = 0;
            this.rBody.velocity = Vector3.zero;
            //this.transform.localPosition = new Vector3( 0, 0.5f, 0);
            Debug.Log("Posição inicial");
            this.inRoad = true;
            this.direction = 0;
            this.transform.rotation = new Quaternion(0, 180, 0, 0);
        }

        // Move the target to a new spot
        //Target.localPosition = new Vector3(Random.value * 8 - 4,
        //0.5f,
        //Random.value * 8 - 4);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        Sensors();
        // Target and Agent positions
        sensor.AddObservation(this.hitDirFront.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirFrontRight.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirFrontRightest.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirFrontLeft.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirFrontLeftest.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirRight.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirRightLeft.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirBack.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirBackRight.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirBackLeft.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirBackRightest.collider.gameObject.layer);
        sensor.AddObservation(this.hitDirBackLeftest.collider.gameObject.layer);
        sensor.AddObservation(this.direction);
        sensor.AddObservation(this.inRoad);

        // Agent velocity
        Debug.Log("Velocidade: " + this.rBody.velocity.magnitude);
        sensor.AddObservation(rBody.velocity.x);
        //sensor.AddObservation(rBody.velocity.z);
    }

    private void Sensors(){
        RaycastHit hit;

        Color red = new Color(1, 0, 0);
        Color green = new Color(0, 1, 0);
        Color blue = new Color(0, 0, 1);

        Vector3 raycastDirFront = front.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirFront, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirFront = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);
        }
        Vector3 raycastDirFrontRight = frontRight.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirFrontRight, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirFrontRight = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);
        }
        Vector3 raycastDirFrontRightest = frontRightest.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirFrontRightest, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirFrontRightest = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);
        }
        Vector3 raycastDirFrontLeft = frontLeft.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirFrontLeft, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirFrontLeft = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);
        }
        Vector3 raycastDirFrontLeftest = frontLeftest.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirFrontLeftest, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirFrontLeftest = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);
        }
        Vector3 raycastDirRight = right.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirRight, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirRight = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);
        }
        Vector3 raycastDirRightLeft= left.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirRightLeft, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirRightLeft = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);

        }
        Vector3 raycastDirBack = back.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirBack, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirBack = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);

        }
        Vector3 raycastDirBackRight = backRight.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirBackRight, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirBackRight = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);

        }
        Vector3 raycastDirBackLeft = backLeft.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirBackLeft, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirBackLeft = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);

        }
        Vector3 raycastDirBackRightest = backRightest.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirBackRightest, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirBackRightest = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);

        }
        Vector3 raycastDirBackLeftest = backLeftest.transform.position - sensorRoot.position;
        if (Physics.Raycast(sensorRoot.position, raycastDirBackLeftest, out hit, sensorLength, ~raycastIgnore))
        {
            hitDirBackLeftest = hit;
            if (hit.collider.gameObject.layer == 6)
                Debug.DrawLine(sensorRoot.position, hit.point, green, 1f);
            else if (hit.collider.gameObject.layer == 7)
                Debug.DrawLine(sensorRoot.position, hit.point, red, 1f);
            else
                Debug.DrawLine(sensorRoot.position, hit.point, blue, 1f);

        }

    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        Vector3 controlSignal = Vector3.zero;

        int control = actionBuffers.DiscreteActions[0];
        if(control == 1 ){
            controlSignal.x = 1;
        }else if(control == 2){
            controlSignal.x = -1;
        }else if(control == 3){
            controlSignal.z = -1;
        }else if(control == 4){
            controlSignal.z = 1;
        }else if(control == 5){
            controlSignal.z = -1;
            controlSignal.x = 1;
        }else if(control == 6){
            controlSignal.z = 1;
            controlSignal.x = 1;
        }else if(control == 7){
            controlSignal.z = -1;
            controlSignal.x = -1;
        }else if(control == 8){
            controlSignal.z = 1;
            controlSignal.x = -1;
        }

        this.HandleMotor(controlSignal.x);
        this.HandleSteering(controlSignal.z);
        this.UpdateWheels();
        //this.rBody.AddForce(controlSignal);

        //get wheels
        bool frw_rua = FrontRightWheel.GetComponent<TerrainDetect>().getRua();
        bool flw_rua = FrontLeftWheel.GetComponent<TerrainDetect>().getRua();
        bool brw_rua = BackRightWheel.GetComponent<TerrainDetect>().getRua();
        bool blw_rua = BackLeftWheel.GetComponent<TerrainDetect>().getRua();

        // Rewards
        //moving forwards
        if( controlSignal.x > 0f){
           reward +=10;
            SetReward(10);
           direction = 1;
        }
        else if(controlSignal.x < 0f){
           reward -=2;
            SetReward(-2);
           direction= -1;
        }
        else{
          reward -= 10;
            SetReward(-10);
          direction=0;
        }

        // Reached target
        if(this.hitDirFront.collider.gameObject.layer == 7 &&
            this.hitDirFrontRight.collider.gameObject.layer == 7 &&
            this.hitDirFrontRightest.collider.gameObject.layer == 7 &&
            this.hitDirFrontLeft.collider.gameObject.layer == 7 &&
            this.hitDirFrontLeftest.collider.gameObject.layer == 7 &&
            this.hitDirRight.collider.gameObject.layer == 7 &&
            this.hitDirRightLeft.collider.gameObject.layer == 7 &&
            this.hitDirBack.collider.gameObject.layer == 7 &&
            this.hitDirBackRight.collider.gameObject.layer == 7 &&
            this.hitDirBackLeft.collider.gameObject.layer == 7 &&
            this.hitDirBackRightest.collider.gameObject.layer == 7 &&
            this.hitDirBackLeftest.collider.gameObject.layer == 7){
            reward += 100;
            SetReward(100);
        }else{
            reward -= 100;
            SetReward(-100);
        }

        if(frw_rua && flw_rua && brw_rua && blw_rua)
        {
            inRoad = true;
            //reward+=100;
            //SetReward(100);
        }else{
            inRoad = false;
            //reward-=50;
            //SetReward(-50);
            inRoadCounter+=1;
        }
        //Debug.Log(inRoadCounter);
        elapsed += Time.deltaTime;




        // Fell off platform
        if (this.transform.localPosition.y < 0 || inRoadCounter > 100 ||(elapsed >= trialTime))
        {
            inRoadCounter = 0;
            elapsed = 0;
            Debug.Log("Finalizado");
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKeyDown(KeyCode.UpArrow)){
            discreteActionsOut[0] = 1;
        }else if(Input.GetKeyDown(KeyCode.DownArrow)){
            discreteActionsOut[0] = 2;
        }else if(Input.GetKeyDown(KeyCode.LeftArrow)){
            discreteActionsOut[0] = 3;
        }else if(Input.GetKeyDown(KeyCode.RightArrow)){
            discreteActionsOut[0] = 4;
        }else if(Input.GetKeyDown(KeyCode.RightArrow) & Input.GetKeyDown(KeyCode.UpArrow)){
            discreteActionsOut[0] = 5;
        }else if(Input.GetKeyDown(KeyCode.LeftArrow) & Input.GetKeyDown(KeyCode.UpArrow)){
            discreteActionsOut[0] = 6;
        }else if(Input.GetKeyDown(KeyCode.RightArrow) & Input.GetKeyDown(KeyCode.DownArrow)){
            discreteActionsOut[0] = 7;
        }else if(Input.GetKeyDown(KeyCode.LeftArrow) & Input.GetKeyDown(KeyCode.DownArrow)){
            discreteActionsOut[0] = 8;
        }


        //this.HandleMotor(discreteActionsOut[0]);
        //this.HandleSteering(discreteActionsOut[]);
        //this.UpdateWheels();
        //continuousActionsOut[2] = Input.GetKey(KeyCode.Space);
    }

    private void HandleMotor(float vertical)
    {
        frontLeftWheelCollider.motorTorque = vertical * motorForce;
        frontRightWheelCollider.motorTorque = vertical * motorForce;
        currentBrakeForce = isBreaking ? brakeForce : 0f;

        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontLeftWheelCollider.brakeTorque = currentBrakeForce;
        frontRightWheelCollider.brakeTorque = currentBrakeForce;
        rearLeftWheelCollider.brakeTorque = currentBrakeForce;
        rearRightWheelCollider.brakeTorque = currentBrakeForce;
    }

    private void HandleSteering(float horizontal)
    {
        currentSteerAngle = maxSteeringAngle * horizontal;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    // Update nas animações das rodas \/
    private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, FrontLeftWheel);
        UpdateSingleWheel(frontRightWheelCollider, FrontRightWheel);
        UpdateSingleWheel(rearLeftWheelCollider, BackLeftWheel);
        UpdateSingleWheel(rearRightWheelCollider, BackRightWheel);
    }

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
