import { zodResolver } from "@hookform/resolvers/zod";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { FormField } from "../components/FormField";
import { useAuth } from "../hooks/useAuth";

const registerSchema = z.object({
  firstName: z.string().trim().min(2, "Use at least 2 characters.").max(50),
  lastName: z.string().trim().min(2, "Use at least 2 characters.").max(50),
  userName: z.string().trim().min(3, "Use at least 3 characters.").max(50).regex(/^[a-zA-Z0-9._-]+$/, "Use letters, numbers, dots, dashes, or underscores."),
  email: z.string().trim().email("Enter a valid email address."),
  password: z.string().min(12, "Use at least 12 characters.").max(128).regex(/[A-Z]/, "Add an uppercase letter.").regex(/[a-z]/, "Add a lowercase letter.").regex(/[0-9]/, "Add a number."),
  confirmPassword: z.string(),
}).refine((values) => values.password === values.confirmPassword, {
  message: "Passwords do not match.",
  path: ["confirmPassword"],
});

type RegisterValues = z.infer<typeof registerSchema>;

export function RegisterPage() {
  const { register: createAccount } = useAuth();
  const [serverError, setServerError] = useState<string | null>(null);
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<RegisterValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: { firstName: "", lastName: "", userName: "", email: "", password: "", confirmPassword: "" },
  });

  const submit = handleSubmit(async ({ confirmPassword: _, ...values }) => {
    setServerError(null);
    try {
      await createAccount(values);
    } catch (error) {
      setServerError(error instanceof Error ? error.message : "Registration failed. Please try again.");
    }
  });

  return (
    <>
      <header className="form-heading">
        <p className="eyebrow">Start building</p>
        <h2>Create your account</h2>
        <p>Set up secure access to your collaborative workspace.</p>
      </header>
      <form onSubmit={submit} noValidate>
        {serverError && <div className="form-alert" role="alert">{serverError}</div>}
        <div className="form-row">
          <FormField label="First name" autoComplete="given-name" error={errors.firstName?.message} {...register("firstName")} />
          <FormField label="Last name" autoComplete="family-name" error={errors.lastName?.message} {...register("lastName")} />
        </div>
        <FormField label="Username" autoComplete="username" placeholder="your.name" error={errors.userName?.message} {...register("userName")} />
        <FormField label="Email address" type="email" autoComplete="email" placeholder="you@company.com" error={errors.email?.message} {...register("email")} />
        <div className="form-row">
          <FormField label="Password" type="password" autoComplete="new-password" error={errors.password?.message} {...register("password")} />
          <FormField label="Confirm password" type="password" autoComplete="new-password" error={errors.confirmPassword?.message} {...register("confirmPassword")} />
        </div>
        <button className="primary-button" type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Creating account…" : "Create account"}
        </button>
      </form>
    </>
  );
}
