import { createAction } from '@reduxjs/toolkit';

export const setCredentials = createAction('auth/setCredentials');
export const clearCredentials = createAction('auth/clearCredentials');
export const updateToken = createAction('auth/updateToken');
