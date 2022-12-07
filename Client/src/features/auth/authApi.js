import { createApi } from '@reduxjs/toolkit/query/react';
import { createBaseQueryWithReauth } from './baseQuery';
import { setCredentials, clearCredentials, updateToken } from './authActions';
import { toast } from 'react-hot-toast';

console.log('API Base URL:', process.env.REACT_APP_API_BASE_URL);

const baseQueryWithReauth = createBaseQueryWithReauth({ clearCredentials, updateToken });

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
          dispatch(setCredentials(data));
          toast.success('Successfully logged in!');
        } catch (err) {
          toast.error(err?.error?.data?.errors[0] || 'Failed to login');
        }
      },
    }),
    register: builder.mutation({
      query: (userData) => ({
        url: '/identity/register',
        method: 'POST',
        body: userData,
      }),
      async onQueryStarted(arg, { queryFulfilled }) {
        try {
          await queryFulfilled;
          toast.success('Registration successful! Please login.');
        } catch (err) {
          toast.error(err?.error?.data?.errors[0] || 'Registration failed');
        }
      },
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
          toast.success('Successfully logged out');
        } catch (err) {
          toast.error(err?.error?.data?.errors[0] || 'Failed to logout');
        }
      },
    }),
    refreshToken: builder.mutation({
      query: ({ email, refreshToken }) => ({
        url: '/identity/refresh',
        method: 'POST',
        body: { email, refreshToken },
      }),
    }),
  }),
});

export const {
  useLoginMutation,
  useRegisterMutation,
  useLogoutMutation,
  useRefreshTokenMutation,
} = authApi;
