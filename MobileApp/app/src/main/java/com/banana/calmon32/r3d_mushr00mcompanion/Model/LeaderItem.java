package com.banana.calmon32.r3d_mushr00mcompanion.Model;


public class LeaderItem {

    public String name, score;
    public Integer position;

    public LeaderItem(Integer _position, String _name, String _score) {
        this.position = _position;
        this.name = _name;
        this.score = _score;
    }

    public String getName() {
        return name;
    }

    public String getScore() {
        return score;
    }

    public Integer getPosition() {
        return position;
    }
}
