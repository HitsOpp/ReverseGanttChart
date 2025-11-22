import { Routes, Route, BrowserRouter } from "react-router-dom";
import { WelcomePage, SubjectPage } from "client/pages";
export const AppRouter = () => (
  <BrowserRouter>
    <Routes>
      <Route path="/" element={<WelcomePage />} />
      <Route path="/subjects" element={<SubjectPage />} />
    </Routes>
  </BrowserRouter>
);
