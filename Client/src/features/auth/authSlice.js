import { createSlice } from '@reduxjs/toolkit';
import { decodeToken } from './../../lib/jwt';
import { setCredentials, clearCredentials, updateToken } from './authActions';

const extractUserFromToken = (token) => {
  if (!token) return null;

  const decoded = decodeToken(token);
  if (!decoded) return null;

  return {
    id: decoded.Id,
    email: decoded.Email,
    firstName: decoded.FirstName,
    lastName: decoded.LastName,
    username: decoded.Username,
    roles: decoded.roles,
  };
};

const loadState = () => {
  try {
    const serializedState = localStorage.getItem('auth');
    if (serializedState === null) {
      return undefined;
    }
    const state = JSON.parse(serializedState);

    if (state.token) {
      state.user = extractUserFromToken(state.token);
    }

    return state;
  } catch (err) {
    return undefined;
  }
};

const saveState = (state) => {
  try {
    const serializedState = JSON.stringify(state);
    localStorage.setItem('auth', serializedState);
  } catch (err) {}
};

const initialState = loadState() || {
  user: null,
  token: null,
  refreshToken: null,
  refreshTokenExpiryTime: null,
  isAuthenticated: false,
};

const authSlice = createSlice({
  name: 'auth',
  initialState,
  reducers: {},
  extraReducers: (builder) => {
    builder
      .addCase(setCredentials, (state, { payload }) => {
        state.token = payload.accessToken;
        state.refreshToken = payload.refreshToken;
        state.refreshTokenExpiryTime = payload.refreshTokenExpiryTime;
        state.user = extractUserFromToken(payload.accessToken);
        state.isAuthenticated = true;
        saveState(state);
      })
      .addCase(clearCredentials, (state) => {
        state.user = null;
        state.token = null;
        state.refreshToken = null;
        state.refreshTokenExpiryTime = null;
        state.isAuthenticated = false;
        localStorage.removeItem('auth');
      })
      .addCase(updateToken, (state, { payload }) => {
        state.token = payload.accessToken;
        state.refreshToken = payload.refreshToken;
        state.refreshTokenExpiryTime = payload.refreshTokenExpiryTime;
        saveState(state);
      });
  },
});

export const selectCurrentUser = (state) => state.auth.user;
export const selectIsAuthenticated = (state) => state.auth.isAuthenticated;
export const selectToken = (state) => state.auth.token;
export const selectRefreshToken = (state) => state.auth.refreshToken;
export const selectRefreshTokenExpiryTime = (state) =>
  state.auth.refreshTokenExpiryTime;

export default authSlice.reducer;
