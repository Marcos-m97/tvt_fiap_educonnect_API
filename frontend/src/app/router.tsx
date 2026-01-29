import { createBrowserRouter } from "react-router-dom";

import Login from "../pages/login/Login";
import AdminHome from "../pages/admin/AdminHome";
import ProfessorHome from "../pages/professor/ProfessorHome";
import AlunoHome from "../pages/aluno/AlunoHome";
import ForgotPassword from "../pages/auth/ForgotPassword";
import ResetPassword from "../pages/auth/ResetPassword";

// import AuthGuard from "../guards/AuthGuard";
import RoleGuard from "../guards/RoleGuard";

export const router = createBrowserRouter([
  // 🔓 ROTAS PÚBLICAS
  {
    path: "/login",
    element: <Login />,
  },
  {
    path: "/forgot-password",
    element: <ForgotPassword />,
  },
  {
    path: "/reset-password",
    element: <ResetPassword />,
  },

  // 🔐 ADMIN (roles 0 e 1)
  {
    path: "/admin",
    element: (
      <RoleGuard allowed={[0, 1]}>
        <AdminHome />
      </RoleGuard>
    ),
  },

  // 🔐 PROFESSOR (role 2)
  {
    path: "/professor",
    element: (
      <RoleGuard allowed={[2]}>
        <ProfessorHome />
      </RoleGuard>
    ),
  },

  // 🔐 ALUNO (role 3)
  {
    path: "/aluno",
    element: (
      <RoleGuard allowed={[3]}>
        <AlunoHome />
      </RoleGuard>
    ),
  },

  // fallback opcional
  {
    path: "*",
    element: <Login />,
  },
]);



// import { createBrowserRouter } from "react-router-dom";
// import Login from "../pages/login/Login";
// import AdminHome from "../pages/admin/AdminHome";
// import ProfessorHome from "../pages/professor/ProfessorHome";
// import AlunoHome from "../pages/aluno/AlunoHome";
// import ForgotPassword from "../pages/auth/ForgotPassword";
// import ResetPassword from "../pages/auth/ResetPassword";

// export const router = createBrowserRouter([
//   { path: "/login", element: <Login /> },
//   { path: "/admin", element: <AdminHome /> },
//   { path: "/professor", element: <ProfessorHome /> },
//   { path: "/aluno", element: <AlunoHome /> },
//   { path: "/forgot-password", element: <ForgotPassword /> },
//   { path: "/reset-password", element: <ResetPassword /> },
// ]);




