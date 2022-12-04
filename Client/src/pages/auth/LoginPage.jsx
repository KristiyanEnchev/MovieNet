import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, Loader } from 'lucide-react';
import { authApi } from '../../features/auth/authApi';
import Input from '../../components/auth/Input';
import { loginSchema } from '../../schemas/loginSchema';
import { useFormik } from 'formik';
import { toFormikValidationSchema } from 'zod-formik-adapter';

const LoginPage = () => {
  const navigate = useNavigate();
  const [login, { isLoading }] = authApi.useLoginMutation();

  const formik = useFormik({
    initialValues: {
      email: '',
      password: '',
    },
    validationSchema: toFormikValidationSchema(loginSchema),
    onSubmit: async (values) => {
      try {
        await login(values).unwrap();
        navigate('/');
      } catch (err) {
        console.error('Failed to login:', err);
      }
    },
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-background px-4">
      <div className="max-w-md w-full bg-card/50 backdrop-blur-xl rounded-2xl shadow-xl overflow-hidden border border-border transform transition-all duration-500 hover:shadow-2xl">
        <div className="p-8">
          <h2 className="text-3xl font-bold mb-6 text-center bg-gradient-to-r from-primary to-primary/70 text-transparent bg-clip-text">
            Welcome Back
          </h2>

          <form onSubmit={formik.handleSubmit}>
            <Input
              icon={Mail}
              type="email"
              name="email"
              placeholder="Email Address"
              value={formik.values.email}
              onChange={formik.handleChange}
              error={formik.touched.email && formik.errors.email}
            />

            <Input
              icon={Lock}
              type="password"
              name="password"
              placeholder="Password"
              value={formik.values.password}
              onChange={formik.handleChange}
              error={formik.touched.password && formik.errors.password}
            />

            <div className="flex items-center mb-6">
              <Link
                to="/forgot-password"
                className="text-sm text-primary hover:underline"
              >
                Forgot password?
              </Link>
            </div>

            <button
              className="w-full py-3 px-4 bg-gradient-to-r from-primary to-primary/80 text-primary-foreground
                font-bold rounded-lg shadow-lg hover:from-primary/90 hover:to-primary/70
                focus:outline-none focus:ring-2 focus:ring-primary/30 focus:ring-offset-2
                focus:ring-offset-background transition-all duration-200 transform hover:scale-[1.02] active:scale-[0.98]"
              type="submit"
              disabled={isLoading}
            >
              {isLoading ? (
                <Loader className="w-6 h-6 animate-spin mx-auto" />
              ) : (
                'Sign In'
              )}
            </button>
          </form>
        </div>
        <div className="px-8 py-4 bg-muted/50 flex justify-center">
          <p className="text-sm text-muted-foreground">
            Don't have an account?{' '}
            <Link to="/signup" className="text-primary hover:underline">
              Sign up
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
