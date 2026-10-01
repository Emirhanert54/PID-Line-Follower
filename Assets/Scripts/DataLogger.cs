using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    public PIDLaneFollower pidCar;

    public float sampleInterval = 0.05f;
    public bool saveOnStop = true;

    private float timer;
    private float sampleTimer;

    private List<string> rows = new List<string>();

    void Start()
    {
        rows.Clear();

        rows.Add("time,error_e,controllerOutput_u,Kp,Ki,Kd,vehicleSpeed,vehicleMass");
    }

    void Update()
    {
        if (pidCar == null)
            return;

        timer += Time.deltaTime;
        sampleTimer += Time.deltaTime;

        if (sampleTimer >= sampleInterval)
        {
            sampleTimer = 0f;

            string row =
                timer.ToString("0.000") + "," +
                pidCar.error_e.ToString("0.000") + "," +
                pidCar.controllerOutput_u.ToString("0.000") + "," +
                pidCar.Kp.ToString("0.000") + "," +
                pidCar.Ki.ToString("0.000") + "," +
                pidCar.Kd.ToString("0.000") + "," +
                pidCar.vehicleSpeed.ToString("0.000") + "," +
                pidCar.vehicleMass.ToString("0.000");

            rows.Add(row);
        }
    }

    void OnApplicationQuit()
    {
        if (saveOnStop)
            SaveCSV();
    }

    void OnDisable()
    {
        if (saveOnStop && Application.isPlaying)
            SaveCSV();
    }

    public void SaveCSV()
    {
        string fileName = "PID_Log_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";

        string path = Path.Combine(Application.dataPath, fileName);

        File.WriteAllLines(path, rows);

        Debug.Log("PID verileri kaydedildi: " + path);
    }
}