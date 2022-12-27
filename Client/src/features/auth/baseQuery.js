import { fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { toast } from 'react-hot-toast';

const BASE_URL = process.env.REACT_APP_API_BASE_URL || 'http://localhost:5069/api';

export const baseQuery = fetchBaseQuery({
  baseUrl: BASE_URL,
  prepareHeaders: (headers, { getState }) => {
    const token = getState().auth.token;
    if (token) {
      headers.set('authorization', `Bearer ${token}`);
    }
    return headers;
  },
});

export const createBaseQueryWithReauth = (actions) => {
  const { clearCredentials, updateToken } = actions;
  
  return async (args, api, extraOptions) => {
    let result = await baseQuery(args, api, extraOptions);

    if (result.error && result.error.status === 401) {
      const state = api.getState();
      const refreshToken = state.auth.refreshToken;
      const user = state.auth.user;

      if (!refreshToken || !user) {
        api.dispatch(clearCredentials());
        toast.error('Session expired. Please login again.');
        return result;
      }

      try {
        const refreshResult = await baseQuery(
          {
            url: '/identity/refresh',
            method: 'POST',
            body: {
              email: user.email,
              refreshToken: refreshToken,
            },
          },
          api,
          extraOptions
        );

        if (refreshResult.data) {
          api.dispatch(updateToken(refreshResult.data));
          result = await baseQuery(args, api, extraOptions);
        } else {
          api.dispatch(clearCredentials());
          toast.error('Session expired. Please login again.');
        }
      } catch (err) {
        api.dispatch(clearCredentials());
        toast.error('Session expired. Please login again.');
      }
    }

    return result;
  };
};
