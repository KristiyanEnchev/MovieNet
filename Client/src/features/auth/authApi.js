import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import { setCredentials, clearCredentials, updateToken } from './authSlice';

console.log('API Base URL:', process.env.REACT_APP_API_BASE_URL);

const baseQuery = fetchBaseQuery({
  baseUrl: process.env.REACT_APP_API_BASE_URL || 'http://localhost:5069/api',
  prepareHeaders: (headers, { getState }) => {
    const token = getState().auth.token;
    if (token) {
      headers.set('authorization', `Bearer ${token}`);
    }
    return headers;
  },
});

const baseQueryWithReauth = async (args, api, extraOptions) => {
  let result = await baseQuery(args, api, extraOptions);

  if (result.error && result.error.status === 401) {
    const refreshToken = api.getState().auth.refreshToken;
    const user = api.getState().auth.user;

    if (refreshToken && user) {
      const refreshResult = await baseQuery(
        {
          url: '/identity/refresh-token',
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
      }
    }
  }

  return result;
};

export const authApi = createApi({
  reducerPath: 'authApi',
  baseQuery: baseQueryWithReauth,
  endpoints: (builder) => ({
    login: builder.mutation({
      query: (credentials) => ({
        url: '/identity/login',
        method: 'POST',
        body: credentials,
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          const { data } = await queryFulfilled;
          dispatch(
            setCredentials({
              accessToken: data.accessToken,
              refreshToken: data.refreshToken,
              refreshTokenExpiryTime: data.refreshTokenExpiryTime,
            })
          );
        } catch (err) {}
      },
    }),
    register: builder.mutation({
      query: (userData) => ({
        url: '/identity/register',
        method: 'POST',
        body: userData,
      }),
    }),
    logout: builder.mutation({
      query: (email) => ({
        url: '/identity/logout',
        method: 'POST',
        body: { email },
      }),
      async onQueryStarted(arg, { dispatch, queryFulfilled }) {
        try {
          await queryFulfilled;
          dispatch(clearCredentials());
        } catch (err) {}
      },
    }),
    refreshToken: builder.mutation({
      query: ({ email, refreshToken }) => ({
        url: '/identity/refresh',
        method: 'POST',
        body: { email, refreshToken },
      }),
    }),
    getMe: builder.query({
      query: () => '/identity/me',
    }),
  }),
});

export const {
  useLoginMutation,
  useRegisterMutation,
  useLogoutMutation,
  useRefreshTokenMutation,
  useGetMeQuery,
} = authApi;
