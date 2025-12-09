import { Routes, Route, Navigate } from "react-router-dom";
import {
  WelcomePage,
  SubjectPage,
  SubjectDetailPage,
  SupportPage,
  ProfilePage,
} from "client/pages";
import { ProtectedRouter } from "./ProtectedRouter";

export const AppRouter = () => {
  return (
    <Routes>
      <Route path="/login" element={<WelcomePage />} />
      <Route
        path="/profile"
        element={
          <ProtectedRouter>
            <ProfilePage />
          </ProtectedRouter>
        }
      />
      <Route
        path="/subjects"
        element={
          <ProtectedRouter>
            <SubjectPage />
          </ProtectedRouter>
        }
      />
      <Route path="/" element={<Navigate to="/login" replace />} />

      <Route
        path="/subjects/:id/:tab?"
        element={
          <ProtectedRouter>
            <SubjectDetailPage />
          </ProtectedRouter>
        }
      />
      <Route
        path="/support"
        element={
          <ProtectedRouter>
            <SupportPage />
          </ProtectedRouter>
        }
      />
    </Routes>
  );
};
