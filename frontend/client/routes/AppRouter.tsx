import { Routes, Route } from "react-router-dom";
import {
  WelcomePage,
  SubjectPage,
  SubjectDetailPage,
  SupportPage,
} from "client/pages";
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
