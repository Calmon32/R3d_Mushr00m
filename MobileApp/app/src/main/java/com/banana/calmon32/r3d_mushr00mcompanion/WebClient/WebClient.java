package com.banana.calmon32.r3d_mushr00mcompanion.WebClient;


import android.content.Context;

import com.franmontiel.persistentcookiejar.PersistentCookieJar;
import com.franmontiel.persistentcookiejar.cache.SetCookieCache;
import com.franmontiel.persistentcookiejar.persistence.SharedPrefsCookiePersistor;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.FormBody;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;

public class WebClient {

    private PersistentCookieJar cookieJar;
    private OkHttpClient client;


    public WebClient(Context context) {
        cookieJar =
                new PersistentCookieJar(new SetCookieCache(), new SharedPrefsCookiePersistor(context));
        client = new OkHttpClient.Builder()
                .cookieJar(cookieJar)
                .build();

    }

    public Call loginpost(String username, String password, Callback callback) {
        String url = "http://10.0.2.2/login";
        RequestBody formBody = new FormBody.Builder()
                .add("username", username)
                .add("password", password)
                .build();

        Request request = new Request.Builder()
                .url(url)
                .post(formBody)
                .build();

        Call call = client.newCall(request);
        call.enqueue(callback);
        return call;
    }

    public Call getUser(String username, Callback callback) {
        String url = "http://10.0.2.2/user/" + username;
        Request request = new Request.Builder()
                .url(url)
                .build();
        Call call = client.newCall(request);
        call.enqueue(callback);
        return call;
    }

    public Call getLeader(Callback callback){
        String url = "http://10.0.2.2/leader";
        Request request = new Request.Builder()
                .url(url)
                .build();
        Call call = client.newCall(request);
        call.enqueue(callback);
        return call;
    }

    public Call poiPost(String username, String password, String poi, Callback callback) {
        String url = "http://10.0.2.2/poi/" + poi;
        RequestBody formBody = new FormBody.Builder()
                .add("username", username)
                .add("password", password)
                .build();

        Request request = new Request.Builder()
                .url(url)
                .post(formBody)
                .build();

        Call call = client.newCall(request);
        call.enqueue(callback);
        return call;
    }

}
