import { Routes, Route } from "react-router-dom";
import { WelcomePage, SubjectPage, SubjectDetailPage } from "client/pages";
import { ProtectedRouter } from "./ProtectedRouter";

export const AppRouter = () => {
  return (
    <Routes>
      <Route path="/login" element={<WelcomePage />} />

      <Route
        path="/subjects"
        element={
          <ProtectedRouter>
            <SubjectPage />
          </ProtectedRouter>
        }
      />

      <Route
        path="/subjects/id=:id"
        element={
          <ProtectedRouter>
            <SubjectDetailPage />
          </ProtectedRouter>
        }
      />

      <Route
        path="/"
        element={
          <ProtectedRouter>
            <SubjectPage />
          </ProtectedRouter>
        }
      />
    </Routes>
  );
};
