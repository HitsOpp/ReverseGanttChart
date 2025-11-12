import { useForm } from "react-hook-form";
import { yupResolver } from "@hookform/resolvers/yup";
import * as yup from "yup";
import axios from "axios";
import { FormBlock, type FormField } from "./FormBlock";

interface LoginForm {
  password: string;
  email: string;
}

const schema = yup.object({
  email: yup.string().email("Некорректный email").required("Введите email"),
  password: yup.string().length(8).required("Введите пароль"),
});

const fields: FormField<LoginForm>[] = [
  { name: "email", placeholder: "Введите email", label: "Email" },
  { name: "password", placeholder: "Введите пароль", label: "Password" },
];

export const LoginForm = () => {
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    reset,
  } = useForm<LoginForm>({
    resolver: yupResolver(schema),
  });

  const onSubmit = handleSubmit(async (data) => {
    await axios.post("/api/feedback", data);
    reset();
    alert("Успешно отправлено!");
  });

  return (
    <FormBlock<LoginForm>
      title="Добро пожаловть"
      fields={fields}
      submitText="войти"
      onSubmit={onSubmit}
      register={register}
      errors={errors}
      isSubmitting={isSubmitting}
    />
  );
};
