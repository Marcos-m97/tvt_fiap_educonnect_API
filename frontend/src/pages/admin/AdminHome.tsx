import {
  Typography,
  Box,
  Card,
  CardContent
} from "@mui/material";
import AppLayout from "../../components/layout/AppLayout";
import PeopleIcon from "@mui/icons-material/People";
import SchoolIcon from "@mui/icons-material/School";
import AssignmentIcon from "@mui/icons-material/Assignment";
import EventIcon from "@mui/icons-material/Event";
import { useNavigate } from "react-router-dom";

export default function AdminHome() {
  const navigate = useNavigate();

  const cards = [
    {
      title: "Gestão de Usuários",
      description: "Administrar administradores, professores e alunos.",
      icon: <PeopleIcon sx={{ fontSize: 48 }} />,
      route: "/admin/usuarios"
    },
    {
      title: "Gestão Acadêmica",
      description: "Gerenciar cursos, turmas e disciplinas.",
      icon: <SchoolIcon sx={{ fontSize: 48 }} />,
      route: "/admin/academico/cursos"
    },
    {
      title: "Matrículas",
      description: "Aprovar, alterar status e baixar comprovantes.",
      icon: <AssignmentIcon sx={{ fontSize: 48 }} />,
      route: "/admin/matriculas"
    },
    {
      title: "Eventos",
      description: "Criar e gerenciar eventos acadêmicos.",
      icon: <EventIcon sx={{ fontSize: 48 }} />,
      route: "/admin/eventos"
    }
  ];

  return (
    <AppLayout>
      <Box maxWidth="1000px" mx="auto">

        {/* HEADER */}
        <Box textAlign="center" mb={6}>
          <Typography
            variant="h3"
            sx={{
              fontWeight: 800,
              letterSpacing: "0.08em",
              mb: 2
            }}
          >
            Painel do{" "}
            <Box component="span" sx={{ color: "primary.main" }}>
              Administrador
            </Box>
          </Typography>

          <Typography
            variant="h6"
            color="text.secondary"
            sx={{
              fontWeight: 400,
              maxWidth: "600px",
              mx: "auto"
            }}
          >
            Gerencie usuários, cursos, matrículas e eventos acadêmicos.
          </Typography>
        </Box>

        {/* GRID */}
        <Box
          display="grid"
          gridTemplateColumns={{
            xs: "1fr",
            md: "1fr 1fr"
          }}
          gap={4}
        >
          {cards.map((card, index) => (
            <Card
              key={index}
              onClick={() => navigate(card.route)}
              sx={{
                p: 4,
                borderRadius: 4,
                border: "1px solid",
                borderColor: "divider",
                cursor: "pointer",
                transition: "all 0.25s ease",
                textAlign: "center",
                "&:hover": {
                  boxShadow: 8,
                  transform: "translateY(-6px)",
                  borderColor: "primary.main"
                }
              }}
            >
              <CardContent>

                <Box
                  display="flex"
                  justifyContent="center"
                  alignItems="center"
                  mb={2}
                  color="primary.main"
                >
                  {card.icon}
                </Box>

                <Typography
                  variant="h5"
                  fontWeight={700}
                  mb={1}
                >
                  {card.title}
                </Typography>

                <Typography
                  variant="body1"
                  color="text.secondary"
                  sx={{ maxWidth: "320px", mx: "auto" }}
                >
                  {card.description}
                </Typography>

              </CardContent>
            </Card>
          ))}
        </Box>

      </Box>
    </AppLayout>
  );
}