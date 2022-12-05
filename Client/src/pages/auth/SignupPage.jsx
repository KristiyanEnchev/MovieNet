import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { Mail, Lock, User, Loader } from 'lucide-react';
import { useRegisterMutation } from '../../features/auth/authApi';
import Input from '../../components/auth/Input';
import registrationSchema from '../../schemas/registrationSchema';
import { useFormik } from 'formik';
import { toFormikValidationSchema } from 'zod-formik-adapter';
import PasswordStrengthBar from 'react-password-strength-bar';

export default function SignupPage() {
  const navigate = useNavigate();
  const [register, { isLoading }] = useRegisterMutation();

  const formik = useFormik({
    initialValues: {
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      confirmPassword: '',
    },
    validationSchema: toFormikValidationSchema(registrationSchema),
    onSubmit: async (values) => {
      try {
        await register(values).unwrap();
        navigate('/login');
      } catch (err) {
        console.error('Registration failed:', err);
      }
    },
  });

  return (
    <div className="min-h-screen flex items-center justify-center bg-background px-4">
      <div className="max-w-md w-full bg-card/50 backdrop-blur-xl rounded-2xl shadow-xl overflow-hidden border border-border transform transition-all duration-500 hover:shadow-2xl">
        <div className="p-8">
          <h2 className="text-3xl font-bold mb-6 text-center bg-gradient-to-r from-primary to-primary/70 text-transparent bg-clip-text">
            Create Account
          </h2>

          <form onSubmit={formik.handleSubmit}>
            <Input
              icon={User}
              type="text"
              name="firstName"
              placeholder="First Name"
              value={formik.values.firstName}
              onChange={formik.handleChange}
              error={formik.touched.firstName && formik.errors.firstName}
            />

            <Input
              icon={User}
              type="text"
              name="lastName"
              placeholder="Last Name"
              value={formik.values.lastName}
              onChange={formik.handleChange}
              error={formik.touched.lastName && formik.errors.lastName}
            />

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

            <div className="mb-4">
              <PasswordStrengthBar password={formik.values.password} />
            </div>

            <Input
              icon={Lock}
              type="password"
              name="confirmPassword"
              placeholder="Confirm Password"
              value={formik.values.confirmPassword}
              onChange={formik.handleChange}
              error={
                formik.touched.confirmPassword && formik.errors.confirmPassword
              }
            />

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
                'Create Account'
              )}
            </button>
          </form>
        </div>
        <div className="px-8 py-4 bg-muted/50 flex justify-center">
          <p className="text-sm text-muted-foreground">
            Already have an account?{' '}
            <Link to="/login" className="text-primary hover:underline">
              Sign in
            </Link>
          </p>
        </div>
      </div>
    </div>
  );
}
