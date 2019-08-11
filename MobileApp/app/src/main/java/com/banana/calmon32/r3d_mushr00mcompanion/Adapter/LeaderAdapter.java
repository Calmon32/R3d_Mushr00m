package com.banana.calmon32.r3d_mushr00mcompanion.Adapter;

import android.app.Activity;
import android.content.Intent;
import android.support.v7.util.DiffUtil;
import android.support.v7.widget.CardView;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.banana.calmon32.r3d_mushr00mcompanion.Model.LeaderItem;
import com.banana.calmon32.r3d_mushr00mcompanion.Model.Stats;
import com.banana.calmon32.r3d_mushr00mcompanion.R;
import com.banana.calmon32.r3d_mushr00mcompanion.UserProfileActivity;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;

class LeaderViewHolder extends RecyclerView.ViewHolder {

    public TextView name, score, position;
    CardView card;

    public LeaderViewHolder(View itemView) {
        super(itemView);
        name = (TextView) itemView.findViewById(R.id.leaderName);
        score = (TextView) itemView.findViewById(R.id.leaderScore);
        position = (TextView) itemView.findViewById(R.id.leaderPosition);
        card = (CardView) itemView.findViewById(R.id.leaderItem);
    }
}

public class LeaderAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

    public ArrayList<LeaderItem> mLeaderItems;
    Activity activity;

    public LeaderAdapter(Activity activity, ArrayList<LeaderItem> leaderItems) {
        this.activity = activity;
        this.mLeaderItems = leaderItems;
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(activity).inflate(R.layout.leader_item_layout, parent, false);
        return new LeaderViewHolder(view);
    }

    @Override
    public void onBindViewHolder(RecyclerView.ViewHolder holder, final int position) {
        Collections.sort(mLeaderItems, new leaderComparator());
        if (holder instanceof LeaderViewHolder) {
            LeaderViewHolder viewHolder = (LeaderViewHolder) holder;
            viewHolder.card.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    Intent i = new Intent(activity,UserProfileActivity.class);
                    i.putExtra("username", mLeaderItems.get(position).getName());
                    activity.startActivity(i);
                }
            });
            viewHolder.position.setText(String.valueOf(mLeaderItems.get(position).getPosition()));
            viewHolder.name.setText(mLeaderItems.get(position).getName());
            viewHolder.score.setText(mLeaderItems.get(position).getScore());
        }
    }

    @Override
    public int getItemCount() {
        return mLeaderItems.size();
    }

}

class leaderComparator implements Comparator<LeaderItem>
{
    public int compare(LeaderItem left, LeaderItem right) {
        return right.score.compareTo(left.score);
    }
}



