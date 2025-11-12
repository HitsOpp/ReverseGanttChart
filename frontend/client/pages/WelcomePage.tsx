import { RegisterForm, SidePanel } from "client/components";

export const WelcomePage = () => {
  return (
    <div className="min-h-screen bg-gray-50">
      <SidePanel userName="Мосин Денис Юрьевич" />

      <main className="lg:ml-80 p-8">
        <div className="max-w-4xl mx-auto">
          <h1 className="text-3xl font-bold text-gray-900 mb-8">
            Главная страница
          </h1>
          <RegisterForm />
        </div>
      </main>
    </div>
  );
};
