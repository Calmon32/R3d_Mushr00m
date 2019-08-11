package com.banana.calmon32.r3d_mushr00mcompanion;

import android.app.Fragment;
import android.os.Bundle;
import android.support.annotation.Nullable;
import android.support.v7.widget.LinearLayoutManager;
import android.support.v7.widget.RecyclerView;
import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.banana.calmon32.r3d_mushr00mcompanion.Adapter.LeaderAdapter;
import com.banana.calmon32.r3d_mushr00mcompanion.Adapter.StatsAdapter;
import com.banana.calmon32.r3d_mushr00mcompanion.Model.LeaderItem;
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

import static com.google.android.gms.internal.zzahn.runOnUiThread;


public class LeaderboardFragment extends Fragment{

    View myView;
    private RecyclerView mRecyclerView;
    private LeaderAdapter mAdapter;
    private RecyclerView.LayoutManager mLayoutManager;
    public ArrayList<LeaderItem> leaderItems = new ArrayList<>();
    WebClient web;

    @Nullable
    @Override
    public View onCreateView(LayoutInflater inflater, @Nullable ViewGroup container, Bundle savedInstanceState) {
        myView = inflater.inflate(R.layout.leader_layout, container, false);
        web = new WebClient(getActivity());

        mRecyclerView = (RecyclerView) myView.findViewById(R.id.leader_recycler_view);
        mRecyclerView.setHasFixedSize(false);
        mLayoutManager = new LinearLayoutManager(getActivity());
        mRecyclerView.setLayoutManager(mLayoutManager);
        mAdapter = new LeaderAdapter(getActivity(), leaderItems);
        mRecyclerView.setAdapter(mAdapter);

        web.getLeader(new Callback() {
            @Override
            public void onFailure(Call call, IOException e) {

            }

            @Override
            public void onResponse(Call call, Response response) throws IOException {
                if (response.isSuccessful()) {
                    final String responseStr = response.body().string();
                    Log.v("Data", responseStr);
                    runOnUiThread(new Runnable() {
                        @Override
                        public void run() {
                            try {
                                final JSONObject jObject = new JSONObject(responseStr);
                                Integer status = jObject.getInt("status");
                                if (status == 200) {
                                    JSONArray arr = jObject.getJSONArray("users");
                                    leaderItems.clear();
                                    for (int i = 0; i < arr.length(); i++) {
                                        JSONArray arr2 = arr.getJSONArray(i);
                                        String name = arr2.getString(0);
                                        String score = arr2.getString(1);
                                        leaderItems.add(new LeaderItem((i + 1), name, score));
                                    }
                                    mAdapter.notifyDataSetChanged();
                                }
                            } catch (JSONException e) {
                                e.printStackTrace();
                            }
                        }
                    });
                } else {
                    Log.v("Debug","Post not successful.");
                }
            }
        });

        return myView;
    }
}
