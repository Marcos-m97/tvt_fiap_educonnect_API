import { createBrowserRouter } from "react-router-dom";
import Login from "../pages/login/Login";
import AdminHome from "../pages/admin/AdminHome";
import ProfessorHome from "../pages/professor/ProfessorHome";
import AlunoHome from "../pages/aluno/AlunoHome";
import ForgotPassword from "../pages/auth/ForgotPassword";
import ResetPassword from "../pages/auth/ResetPassword";

export const router = createBrowserRouter([
  { path: "/login", element: <Login /> },
  { path: "/admin", element: <AdminHome /> },
  { path: "/professor", element: <ProfessorHome /> },
  { path: "/aluno", element: <AlunoHome /> },
  { path: "/forgot-password", element: <ForgotPassword /> },
  { path: "/reset-password", element: <ResetPassword /> },
]);




