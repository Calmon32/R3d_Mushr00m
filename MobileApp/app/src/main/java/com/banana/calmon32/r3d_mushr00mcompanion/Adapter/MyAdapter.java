package com.banana.calmon32.r3d_mushr00mcompanion.Adapter;

import android.app.Activity;
import android.content.ActivityNotFoundException;
import android.content.ClipData;
import android.content.Context;
import android.content.SharedPreferences;
import android.support.v7.util.DiffUtil;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

import com.banana.calmon32.r3d_mushr00mcompanion.MapDiffCallback;
import com.banana.calmon32.r3d_mushr00mcompanion.Model.MapItem;
import com.banana.calmon32.r3d_mushr00mcompanion.Model.Stats;
import com.banana.calmon32.r3d_mushr00mcompanion.R;
import com.banana.calmon32.r3d_mushr00mcompanion.WebClient.WebClient;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.Response;

import static com.google.android.gms.internal.zzahn.runOnUiThread;

class ItemViewHolder extends RecyclerView.ViewHolder {

    public TextView name, position;
    public Button bttn;

    public ItemViewHolder(View itemView) {
        super(itemView);
        bttn = (Button) itemView.findViewById(R.id.openbttn);
        name = (TextView) itemView.findViewById(R.id.textName);
        position = (TextView) itemView.findViewById(R.id.textPosition);
    }
}

public class MyAdapter extends RecyclerView.Adapter<RecyclerView.ViewHolder> {

    public ArrayList<MapItem> mMapItems;
    Activity activity;
    WebClient web;

    public MyAdapter(Activity activity, ArrayList<MapItem> mapItems) {
        this.activity = activity;
        this.mMapItems = mapItems;
        web = new WebClient(activity.getApplicationContext());
    }

    @Override
    public RecyclerView.ViewHolder onCreateViewHolder(ViewGroup parent, int viewType) {
        View view = LayoutInflater.from(activity).inflate(R.layout.map_item_layout, parent, false);
        return new ItemViewHolder(view);
    }

    @Override
    public void onBindViewHolder(RecyclerView.ViewHolder holder, final int position) {
        if (holder instanceof ItemViewHolder) {
            MapItem mapitem = mMapItems.get(position);
            ItemViewHolder viewHolder = (ItemViewHolder) holder;
            viewHolder.name.setText(mMapItems.get(position).getName());
            String dist = String.valueOf(Math.round(mMapItems.get(position).getDistance())) + " meters";

            SharedPreferences sharedPref = activity.getSharedPreferences("CRED", Context.MODE_PRIVATE);
            final String user = sharedPref.getString("username", null);
            final String pass = sharedPref.getString("password", null);

            viewHolder.position.setText(dist);
            viewHolder.bttn.setOnClickListener(new View.OnClickListener() {
                public void onClick(View v) {
                    if (mMapItems.get(position).getDistance() <= 20) {
                        web.poiPost(user, pass, mMapItems.get(position).getId(), new Callback() {
                            @Override
                            public void onFailure(Call call, IOException e) {

                            }

                            @Override
                            public void onResponse(Call call, Response response) throws IOException {
                                if (response.isSuccessful()) {
                                    String responseStr = response.body().string();
                                    Log.v("Data", responseStr);
                                    try {
                                        final JSONObject jObject = new JSONObject(responseStr);
                                        Integer status = jObject.getInt("status");
                                        if (status == 200) {
                                            Integer item  = jObject.getInt("poiget");
                                            final String str = "Got item: " + String.valueOf(item);
                                            runOnUiThread(new Runnable() {
                                                @Override
                                                public void run() {
                                                    Toast.makeText(activity, str, Toast.LENGTH_LONG).show();
                                                }
                                            });
                                            Log.v("Item", String.valueOf(item));
                                        } else if (status == 201) {
                                            final String mess = jObject.getString("message");
                                            Log.v("Debug",mess);
                                            runOnUiThread(new Runnable() {
                                                @Override
                                                public void run() {
                                                    Toast.makeText(activity, mess, Toast.LENGTH_LONG).show();
                                                }
                                            });
                                        }
                                    } catch (JSONException e) {
                                        e.printStackTrace();
                                    }
                                } else {
                                    Log.v("Debug","Post not successful.");
                                }
                            }
                        });
                    } else {
                        Toast.makeText(activity, "Too far away! Come closer ;)", Toast.LENGTH_LONG).show();
                    }
                }
            });
        }
    }

    @Override
    public int getItemCount() {
        return mMapItems.size();
    }

    public void updateMapItems(ArrayList<MapItem> _mapItems) {
            final MapDiffCallback diffCallback = new MapDiffCallback(this.mMapItems, _mapItems);
            final DiffUtil.DiffResult diffResult = DiffUtil.calculateDiff(diffCallback);
            this.mMapItems.clear();
            this.mMapItems.addAll(_mapItems);
            diffResult.dispatchUpdatesTo(this);
    }
}


