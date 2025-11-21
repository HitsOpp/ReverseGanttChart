import { useState } from "react";
import { LoginForm, RegisterForm } from "client/components";

export const WelcomePage = () => {
  const [modalType, setModalType] = useState<"login" | "register" | null>(null);

  const openLoginModal = () => setModalType("login");
  const openRegisterModal = () => setModalType("register");
  const closeModal = () => setModalType(null);

  return (
    <div className="min-h-screen bg-linear-to-br from-indigo-50 via-white to-blue-100 flex flex-col justify-between">
      <section className="flex flex-col items-center justify-center grow text-center px-6 py-24">
        <h1 className="text-6xl font-extrabold bg-clip-text text-transparent bg-linear-to-r from-indigo-600 to-blue-600 mb-6">
          Добро пожаловать
        </h1>
        <p className="text-xl text-gray-600 max-w-2xl mb-10 leading-relaxed">
          Платформа для студентов и преподавателей, где знания встречаются с
          технологиями. Учитесь, делитесь и развивайтесь вместе.
        </p>

        <div className="flex flex-col sm:flex-row gap-4">
          <button
            onClick={openLoginModal}
            className="px-8 py-3 cursor-pointer bg-indigo-600 text-white rounded-xl font-semibold text-lg
                       shadow-lg hover:bg-indigo-700 transition-all duration-200"
          >
            Войти
          </button>
          <button
            onClick={openRegisterModal}
            className="px-8 py-3 cursor-pointer  bg-white border border-indigo-200 text-indigo-700 rounded-xl font-semibold text-lg
                       shadow-sm hover:border-indigo-400 hover:bg-indigo-50 transition-all duration-200"
          >
            Зарегистрироваться
          </button>
        </div>
      </section>

      <footer className="py-8 text-center text-sm text-gray-500">
        © {new Date().getFullYear()} StudentsTasks — обучение нового поколения
      </footer>

      {modalType && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50 px-4">
          <div className="bg-white rounded-2xl shadow-2xl max-w-md w-full relative animate-fadeIn">
            <button
              onClick={closeModal}
              className="absolute cursor-pointer top-4 right-4 text-gray-400 hover:text-gray-600 transition-colors"
            >
              ✕
            </button>

            <div className="p-6">
              {modalType === "login" ? <LoginForm /> : <RegisterForm />}
            </div>

            <div className="p-4 text-center text-sm text-gray-600">
              {modalType === "login" ? (
                <>
                  Нет аккаунта?
                  <button
                    onClick={openRegisterModal}
                    className="ml-1 cursor-pointer text-indigo-600 font-semibold hover:text-indigo-700 underline"
                  >
                    Зарегистрироваться
                  </button>
                </>
              ) : (
                <>
                  Уже есть аккаунт?
                  <button
                    onClick={openLoginModal}
                    className="ml-1 cursor-pointers text-indigo-600 font-semibold hover:text-indigo-700 underline"
                  >
                    Войти
                  </button>
                </>
              )}
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
