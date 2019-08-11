package com.banana.calmon32.r3d_mushr00mcompanion.Model;

import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.Marker;

public class MapItem {
    public String name;
    public float distance;
    public LatLng position;
    public String id;
    public Marker marker;

    public MapItem(String _name, String _id, float _distance, LatLng _position, Marker _marker) {
        this.name = _name;
        this.position = _position;
        this.marker = _marker;
        this.distance = _distance;
        this.id = _id;
    }

    public String getId() {
        return id;
    }

    public Marker getMarker() {
        return marker;
    }

    public String getName() {
        return name;
    }

    public float getDistance() {
        return distance;
    }

    public LatLng getPosition() {
        return position;
    }

    public void destroyMarker() {
        if (marker != null) marker.remove();
    }
}
