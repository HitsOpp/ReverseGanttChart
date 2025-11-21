import { Routes, Route } from "react-router-dom";
import { WelcomePage, SubjectPage } from "client/pages";
export const AppRouter = () => (
  <Routes>
    <Route path="/" element={<WelcomePage />} />
    <Route path="/subjects" element={<SubjectPage />} />
  </Routes>
);
