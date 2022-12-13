import { configureStore, combineReducers } from '@reduxjs/toolkit';
import { createLogger } from 'redux-logger';
import { mediaApi } from '../features/media/mediaApi';
import { authApi } from '../features/auth/authApi';
import authReducer from '../features/auth/authSlice';

const logger = createLogger({
  collapsed: true,
  diff: true,
});

const combinedReducer = combineReducers({
  [mediaApi.reducerPath]: mediaApi.reducer,
  [authApi.reducerPath]: authApi.reducer,
  auth: authReducer,
});

const rootReducer = (state, action) => {
  if (action.type === 'auth/clearCredentials') {
    state = undefined;
    mediaApi.util.resetApiState();
  }
  return combinedReducer(state, action);
};

export const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware({
      serializableCheck: false,
      immutableCheck: false,
    })
      .concat(mediaApi.middleware)
      .concat(authApi.middleware)
      .concat(process.env.NODE_ENV !== 'production' ? logger : []),
  devTools: process.env.NODE_ENV !== 'production',
});
