package com.banana.calmon32.r3d_mushr00mcompanion;

import android.content.Intent;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.app.AppCompatActivity;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.widget.TextView;

import com.banana.calmon32.r3d_mushr00mcompanion.Adapter.StatsAdapter;
import com.banana.calmon32.r3d_mushr00mcompanion.Model.Stats;
import com.banana.calmon32.r3d_mushr00mcompanion.WebClient.WebClient;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.util.ArrayList;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.Response;



public class UserProfileActivity extends AppCompatActivity {

    WebClient web;
    private RecyclerView mRecyclerView;
    private StatsAdapter mAdapter;
    private RecyclerView.LayoutManager mLayoutManager;
    public ArrayList<Stats> statsList = new ArrayList<>();

    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.user_profile_layout);

        web = new WebClient(this);

        Intent intent = getIntent();
        String user = intent.getStringExtra("username");

        mRecyclerView = (RecyclerView) findViewById(R.id.stats_recycler_view);
        mRecyclerView.setHasFixedSize(false);
        mLayoutManager = new LinearLayoutManager(this);
        mRecyclerView.setLayoutManager(mLayoutManager);
        mAdapter = new StatsAdapter(this, statsList);
        mRecyclerView.setAdapter(mAdapter);
        web.getUser(user, new Callback() {
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
                            runOnUiThread(new Runnable() {
                                @Override
                                public void run() {
                                    try {
                                        TextView username = (TextView) findViewById(R.id.statsUsername);
                                        //TextView navuser = getActivity().findViewById(R.id.navUsername);
                                        //navuser.setText(jObject.getString("username"));
                                        username.setText(jObject.getString("username")+"'s stats");
                                        JSONArray arr = jObject.getJSONArray("stats");
                                        statsList.clear();
                                        for (int i = 0; i < arr.length(); i++) {
                                            JSONArray arr2 = arr.getJSONArray(i);
                                            String name = arr2.getString(0);
                                            String value = arr2.getString(1);
                                            statsList.add(new Stats(name,value));
                                            Log.v("STATS", name + " : " + value);
                                        }
                                        mAdapter.notifyDataSetChanged();
                                    } catch (JSONException e) {
                                        e.printStackTrace();
                                    }
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
    }
}
