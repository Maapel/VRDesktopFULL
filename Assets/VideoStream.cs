using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System;
using System.IO;
using System.Collections.Concurrent;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System.Net;

class QueueItem {
    public int ind ;
    public byte[] data;
    public QueueItem(byte[] data , int ind)
    {
        this.data = data ;  
        this.ind = ind ;    
    }
}
public class VideoStream: MonoBehaviour
{

    public int screens = 2;
    public string camIP = "192.168.0.103";
    public int camPort = 9999;
    public GameObject screen_prefab = null;
    Thread m_NetworkThread;
    bool m_NetworkRunning;
    ConcurrentQueue<QueueItem> dataQueue = new ConcurrentQueue<QueueItem>();
    int[] lengths;
    private void Awake()
    {
        //camIP = GetLocalIPAddress();
        lengths = new int[screens];
    }
    private void OnEnable()
    {
        m_NetworkRunning = true;
        m_NetworkThread = new Thread(NetworkThread);
        m_NetworkThread.Start();

    }
    private void OnDisable()
    {
        m_NetworkRunning = false;
        if (m_NetworkThread != null)
        {
            if (!m_NetworkThread.Join(100))
            {
                m_NetworkThread.Abort();
            }
        }
    }
    private void NetworkThread()
    {
        TcpClient client = new TcpClient();
        client.Connect(camIP, camPort);
        Debug.Log("Conected");
        Debug.Log(client.GetStream().ToString());
        using (var stream = client.GetStream())
        {
            BinaryReader reader = new BinaryReader(stream);
            /*try
            {*/
                Debug.Log(client.Connected);
                Debug.Log(stream.CanRead);
                Debug.Log(m_NetworkRunning);

                byte[] data;
                while (m_NetworkRunning && client.Connected && stream.CanRead)
                {
                   
                    
                    for (int j = 0; j < screens; j++)
                    {
                        lengths[j] = reader.ReadInt32();

                        Debug.Log(j);
                        Debug.Log(lengths[j]);

                        
                    }
                    for (int j = 0; j < screens; j++)
                    {
                        data = reader.ReadBytes(lengths[j]);
                        QueueItem queue_item = new QueueItem(data,j);
                        Debug.Log(BitConverter.ToString(data));
                        dataQueue.Enqueue(queue_item);
                    }

                }
           /* }
            catch (Exception e)
            {
                Debug.Log(e.ToString());    
            }*/
        }
    }

    public List<Material> materials;

    public List<Texture2D> textures = new List<Texture2D>();

    void Update()
    {
            QueueItem data ;

        for (int i = 0; i < dataQueue.Count; i++)
        {
            
            if (dataQueue.Count > 0 && dataQueue.TryDequeue(out data))
            {
                Debug.Log(i.ToString());
                Debug.Log(dataQueue.Count.ToString());

                if (data.ind >= textures.Count)
                {
                    var obj = Instantiate(screen_prefab);
                    materials.Add(obj.transform.GetChild(0).GetComponent<Renderer>().material);
                    textures.Add(new Texture2D(1, 1));
                }
                textures[data.ind].LoadImage(data.data);
                textures[data.ind].Apply();
                materials[data.ind].mainTexture = textures[data.ind];
            }
        }
    }
    public string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        Debug.Log(host.HostName);
        foreach (var ip in host.AddressList)
        {
            Debug.Log(ip.ToString());
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {

                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }
}