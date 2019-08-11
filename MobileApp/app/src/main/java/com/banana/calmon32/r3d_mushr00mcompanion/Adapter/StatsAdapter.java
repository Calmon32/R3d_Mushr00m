package com.banana.calmon32.r3d_mushr00mcompanion.Adapter;

import android.app.Activity;
import android.support.v7.util.DiffUtil;
import android.support.v7.widget.RecyclerView;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;

import com.banana.calmon32.r3d_mushr00mcompanion.Model.Stats;
import com.banana.calmon32.r3d_mushr00mcompanion.R;

import java.util.ArrayList;

class StatsViewHolder extends RecyclerView.ViewHolder {

    public TextView name, value;

    public StatsViewHolder(View itemView) {
        super(itemView);
        name = (TextView) itemView.findViewById(R.id.statsName);
        value = (TextView) itemView.findViewById(R.id.statsValue);
    }
}

public class StatsAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

    public ArrayList<Stats> mStatsItems;
    Activity activity;

    public StatsAdapter(Activity activity, ArrayList<Stats> statsItems) {
        this.activity = activity;
        this.mStatsItems = statsItems;
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(activity).inflate(R.layout.stats_item_layout, parent, false);
        return new StatsViewHolder(view);
    }

    @Override
    public void onBindViewHolder(RecyclerView.ViewHolder holder, int position) {
        if (holder instanceof StatsViewHolder) {
            StatsViewHolder viewHolder = (StatsViewHolder) holder;
            viewHolder.name.setText(mStatsItems.get(position).getName());
            viewHolder.value.setText(mStatsItems.get(position).getValue());
        }
    }

    @Override
    public int getItemCount() {
        return mStatsItems.size();
    }

}



