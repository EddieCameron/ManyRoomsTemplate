﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// for tree traversal of all the rooms in the world
public class Room : MonoBehaviour
{
    public string roomName = "REPLACE";
    public string authorName = "ME";

	List<ConnectionPoint> connections = new List<ConnectionPoint>();

	void Awake() {
		// build the list of exits / connection points.
		// there must be an equal number of each
		foreach( var cp in GetComponentsInChildren<ConnectionPoint>() )
		{
            if ( cp.isUsed ) {
                cp.owner = this;
                connections.Add( cp );
            }
		}

        Validate();
	}

    void Validate() {
        if ( connections.Count == 0 )
            Debug.LogError( roomName + " : No connections marked as inUse" );

        int exits = 0;
        int entrances = 0;

        foreach ( var connection in connections ) {
            if ( connection.doorType == ConnectionPoint.DoorType.ENTRANCE )
                entrances++;
            else if ( connection.doorType == ConnectionPoint.DoorType.EXIT )
                exits++;

            if ( Mathf.Abs( connection.connectionX - connection.transform.localPosition.x ) > Mathf.Epsilon ||
                Mathf.Abs( connection.connectionZ - connection.transform.localPosition.z ) > Mathf.Epsilon )
                Debug.LogError( roomName + " : Exit was moved in the x or z dimensions" );
        }
        if ( exits == connections.Count || entrances == connections.Count )
            Debug.LogError( roomName + " :Connections are all marked as entrances or all marked as exits" );
    }

    public ConnectionPoint[] GetConnections() {
        return connections.ToArray();
    }

    public Room[] GetConnectedRooms() {
        var rooms = new List<Room>();
        foreach ( var connector in connections ) {
            if ( connector.isConnected )
                rooms.Add( connector.connectedTo );
        }
        return rooms.ToArray();   
    }

	public ConnectionPoint GetOpenConnection()
	{
        foreach ( var point in connections ) {
            if ( !point.isConnected )
                return point;
        }

		return null;
	}

	public void ConnectRoom( ConnectionPoint cp, Connector connector, Room connectedRoom ){
		if ( cp.isConnected )
            Debug.LogError( "Already room connected at " + cp );

        if ( !connections.Contains( cp ) )
            Debug.LogError( this + " doesn't contain connection " + cp );

        cp.connectedTo = connectedRoom;
        cp.connector = connector;
	}

    public void DisconnectRoom( Room connectedRoom ) {
        foreach ( var connection in connections ) {
            if ( connection.connectedTo == connectedRoom ) {
                connection.connectedTo = null;
                if ( connection.connector ) {
                    Destroy( connection.connector.gameObject );
                    connection.connector = null;
                }
                return;
            }
        }
        Debug.LogWarning( "Couldn't find room " + connectedRoom + " to remove" );
    }
}
