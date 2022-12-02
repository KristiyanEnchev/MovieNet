import { configureStore, combineReducers } from '@reduxjs/toolkit';
import { authApi } from '../features/auth/authApi';
import authReducer from '../features/auth/authSlice';

const combinedReducer = combineReducers({
  [authApi.reducerPath]: authApi.reducer,
  auth: authReducer,
});

const rootReducer = (state, action) => {
  if (action.type === 'auth/clearCredentials') {
    state = undefined;
    authApi.util.resetApiState();
  }
  return combinedReducer(state, action);
};

export const store = configureStore({
  reducer: rootReducer,
  middleware: (getDefaultMiddleware) =>
    getDefaultMiddleware().concat(authApi.middleware),
  devTools: true,
});
