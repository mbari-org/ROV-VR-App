using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;

public class NetworkDebugger : MonoBehaviour
{
    public DataCanvas outputCanvas;

    // Start is called before the first frame update
    void Start()
    {
        DisplayDnsConfiguration();
        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
        IPEndPoint[] tcpEndPoints = properties.GetActiveTcpListeners();
        IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();
        TcpConnectionInformation[] activeEndPoints = properties.GetActiveTcpConnections();


        //Check port for each etc
        //print(tcpEndPoints.Port);
        //print(udpEndPoints.Port);
        //print(tcpEndPoints.Port);

        foreach (IPEndPoint endpoint in udpEndPoints)
        {
            if (endpoint.Port == 7667)
            {
                outputCanvas.printDebug("Address");
                outputCanvas.printDebug(endpoint.Address.ToString());
                outputCanvas.printDebug("Port");
                outputCanvas.printDebug(endpoint.Port.ToString());
            }

        }

        //foreach (IPEndPoint endpoint in tcpEndPoints)
        //{
        //    print("Address");
        //    print(endpoint.Address);
        //    print("Port");
        //    print(endpoint.Port);
        //}
        ////activeEndPoints.LocalEndPoint.Port
        //print("UDP!!");
        //foreach (IPEndPoint endpoint in udpEndPoints)
        //{
        //    print("Address");
        //    print(endpoint.Address);
        //    print("Port");
        //    print(endpoint.Port);
        //}


    }


    // Update is called once per frame
    void Update()
    {

    }

    public void DisplayDnsConfiguration()
    {
        NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
        foreach (NetworkInterface adapter in adapters)
        {
            IPInterfaceProperties properties = adapter.GetIPProperties();
            outputCanvas.printDebug(adapter.Description.ToString());
            //print(properties.DnsSuffix);
            //print(properties.IsDnsEnabled);
            //print(properties.IsDynamicDnsEnabled);
            MulticastIPAddressInformationCollection allinfo = properties.MulticastAddresses;

            //print("HERE!!!");
            //print(allinfo.Count);
            //foreach (IPAddressInformation info in allinfo)
            //{

            //    print(info.Address);

            //}
            //adapter.
            //print(DNS suffix"
            //    properties.DnsSuffix);
            //print("  DNS enabled ............................. : {0}",
            //    properties.IsDnsEnabled);
            //print("  Dynamically configured DNS .............. : {0}",
            //    properties.IsDynamicDnsEnabled);
        }
    }
}
