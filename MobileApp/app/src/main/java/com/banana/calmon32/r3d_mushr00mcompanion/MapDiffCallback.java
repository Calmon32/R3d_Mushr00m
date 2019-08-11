package com.banana.calmon32.r3d_mushr00mcompanion;


import android.support.annotation.Nullable;
import android.support.v7.util.DiffUtil;

import com.banana.calmon32.r3d_mushr00mcompanion.Model.MapItem;

import java.util.List;

public class MapDiffCallback extends DiffUtil.Callback {

    private final List<MapItem> mOldMapList;
    private final List<MapItem> mNewMapList;

    public MapDiffCallback(List<MapItem> oldEmployeeList, List<MapItem> newEmployeeList) {
        this.mOldMapList = oldEmployeeList;
        this.mNewMapList = newEmployeeList;
    }

    @Override
    public int getOldListSize() {
        return mOldMapList.size();
    }

    @Override
    public int getNewListSize() {
        return mNewMapList.size();
    }

    @Override
    public boolean areItemsTheSame(int oldItemPosition, int newItemPosition) {
        return mOldMapList.get(oldItemPosition).getPosition().equals(mNewMapList.get(
                newItemPosition).getPosition());
    }

    @Override
    public boolean areContentsTheSame(int oldItemPosition, int newItemPosition) {
        final MapItem oldEmployee = mOldMapList.get(oldItemPosition);
        final MapItem newEmployee = mNewMapList.get(newItemPosition);

        return oldEmployee.getName().equals(newEmployee.getName());
    }

    @Nullable
    @Override
    public Object getChangePayload(int oldItemPosition, int newItemPosition) {
        // Implement method if you're going to use ItemAnimator
        return super.getChangePayload(oldItemPosition, newItemPosition);
    }
}