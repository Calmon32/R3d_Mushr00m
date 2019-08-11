package com.banana.calmon32.r3d_mushr00mcompanion;

import android.app.Fragment;
import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.design.widget.FloatingActionButton;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.TextView;


import com.banana.calmon32.r3d_mushr00mcompanion.Adapter.MyAdapter;
import com.banana.calmon32.r3d_mushr00mcompanion.Adapter.StatsAdapter;
import com.banana.calmon32.r3d_mushr00mcompanion.Model.Stats;
import com.banana.calmon32.r3d_mushr00mcompanion.WebClient.WebClient;
import com.franmontiel.persistentcookiejar.ClearableCookieJar;
import com.franmontiel.persistentcookiejar.PersistentCookieJar;
import com.franmontiel.persistentcookiejar.cache.SetCookieCache;
import com.franmontiel.persistentcookiejar.persistence.SharedPrefsCookiePersistor;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.w3c.dom.Text;

import java.io.IOException;
import java.util.ArrayList;
import java.util.concurrent.TimeUnit;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.Response;

import static com.google.android.gms.internal.zzahn.runOnUiThread;


/**
 * Created by Lucas on 11/17/2017.
 */

public class ProfileFragment extends Fragment{

    View myView;
    WebClient web;
    private RecyclerView mRecyclerView;
    private StatsAdapter mAdapter;
    private RecyclerView.LayoutManager mLayoutManager;
    public ArrayList<Stats> statsList = new ArrayList<>();

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, Bundle savedInstanceState) {
        myView = inflater.inflate(R.layout.profile_layout, container, false);
        FloatingActionButton myFab = (FloatingActionButton)  myView.findViewById(R.id.mapFab);
        myFab.setOnClickListener(new View.OnClickListener() {
            public void onClick(View v) {
                startActivity(new Intent(getActivity(), MapsActivity.class));
            }
        });


        mRecyclerView = (RecyclerView) myView.findViewById(R.id.stats_recycler_view);

        // use this setting to improve performance if you know that changes
        // in content do not change the layout size of the RecyclerView
        mRecyclerView.setHasFixedSize(false);

        // use a linear layout manager
        mLayoutManager = new LinearLayoutManager(getActivity());
        mRecyclerView.setLayoutManager(mLayoutManager);
        // specify an adapter (see also next example)
        mAdapter = new StatsAdapter(getActivity(), statsList);
        mRecyclerView.setAdapter(mAdapter);

        web = new WebClient(getActivity().getApplicationContext());
        SharedPreferences sharedPref = getActivity().getSharedPreferences("CRED", Context.MODE_PRIVATE);
        String user = sharedPref.getString("username", null);
        String pass = sharedPref.getString("password", null);
                web.loginpost(user, pass, new Callback() {
                    @Override
                    public void onFailure(Call call, IOException e) {
                        Log.v("Debug","Post failed.");
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
                                                TextView user = myView.findViewById(R.id.username);
                                                //TextView navuser = getActivity().findViewById(R.id.navUsername);
                                                //navuser.setText(jObject.getString("username"));
                                                user.setText(jObject.getString("username")+"'s stats");
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

        return myView;
    }

}
