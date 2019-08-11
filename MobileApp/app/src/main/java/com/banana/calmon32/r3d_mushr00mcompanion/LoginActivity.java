package com.banana.calmon32.r3d_mushr00mcompanion;

import android.content.Context;
import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Debug;
import android.support.v7.app.AppCompatActivity;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;

import com.banana.calmon32.r3d_mushr00mcompanion.WebClient.WebClient;
import com.franmontiel.persistentcookiejar.ClearableCookieJar;
import com.franmontiel.persistentcookiejar.PersistentCookieJar;
import com.franmontiel.persistentcookiejar.cache.SetCookieCache;
import com.franmontiel.persistentcookiejar.persistence.SharedPrefsCookiePersistor;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.net.CookieHandler;
import java.net.CookieManager;
import java.net.CookiePolicy;

import okhttp3.Call;
import okhttp3.Callback;
import okhttp3.FormBody;
import okhttp3.MediaType;
import okhttp3.MultipartBody;
import okhttp3.OkHttpClient;
import okhttp3.Request;
import okhttp3.RequestBody;
import okhttp3.Response;
import okhttp3.internal.JavaNetCookieJar;

public class LoginActivity extends AppCompatActivity {

    String pass, user;
    EditText passIn;
    EditText userIn;
    public static final MediaType JSON = MediaType.parse("application/json; charset=utf-8");
    WebClient web;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_login);
        userIn = (EditText) findViewById(R.id.userinput);
        passIn = (EditText) findViewById(R.id.passinput);
        web = new WebClient(getApplicationContext());
    }

    public void checkLogin(View view) throws IOException, JSONException {
        user = userIn.getText().toString();
        pass = passIn.getText().toString();
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
                        JSONObject jObject = new JSONObject(responseStr);
                        Integer status = jObject.getInt("status");
                        if (status == 200) {
                            SharedPreferences sharedPref = getSharedPreferences("CRED",Context.MODE_PRIVATE);
                            SharedPreferences.Editor editor = sharedPref.edit();
                            editor.putString("username", user);
                            editor.putString("password", pass);
                            editor.apply();
                            startActivity(new Intent(LoginActivity.this, MainActivity.class));
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
