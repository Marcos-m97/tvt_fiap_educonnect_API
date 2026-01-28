import { createBrowserRouter } from "react-router-dom";
import LoginColaborador from "../pages/login/LoginColaborador";
import LoginAluno from "../pages/login/LoginAluno";

export const router = createBrowserRouter([
  {
    path: "/login",
    element: <LoginColaborador />,
  },
  {
    path: "/login-aluno",
    element: <LoginAluno />,
  },
]);
