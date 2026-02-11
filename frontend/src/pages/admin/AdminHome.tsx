import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button
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
      icon: <PeopleIcon fontSize="large" />,
      route: "/admin/usuarios"
    },
    {
      title: "Estrutura Acadêmica",
      description: "Gerenciar cursos, disciplinas e turmas.",
      icon: <SchoolIcon fontSize="large" />,
      route: "/admin/academico"
    },
    {
      title: "Matrículas",
      description: "Aprovar, alterar status e baixar comprovantes.",
      icon: <AssignmentIcon fontSize="large" />,
      route: "/admin/matriculas"
    },
    {
      title: "Eventos e Boletins",
      description: "Criar eventos acadêmicos e boletins.",
      icon: <EventIcon fontSize="large" />,
      route: "/admin/eventos"
    }
  ];

  return (
    <AppLayout>
      <Typography variant="h4" gutterBottom>
        Painel do Administrador
      </Typography>

      <Box
        mt={3}
        display="grid"
        gridTemplateColumns={{
          xs: "1fr",
          md: "1fr 1fr"
        }}
        gap={3}
      >
        {cards.map((card, index) => (
          <Card
            key={index}
            sx={{
              height: "100%",
              transition: "0.2s",
              "&:hover": {
                boxShadow: 6,
                transform: "translateY(-4px)"
              }
            }}
          >
            <CardContent>
              <Box display="flex" alignItems="center" gap={2}>
                {card.icon}
                <Typography variant="h6">
                  {card.title}
                </Typography>
              </Box>

              <Typography variant="body2" mt={2}>
                {card.description}
              </Typography>
            </CardContent>

            <CardActions>
              <Button
                size="small"
                onClick={() => navigate(card.route)}
              >
                Acessar
              </Button>
            </CardActions>
          </Card>
        ))}
      </Box>
    </AppLayout>
  );
}
