import {
  Typography,
  Box,
  Card,
  CardContent,
  CardActions,
  Button
} from "@mui/material";
import AppLayout from "../../components/layout/AppLayout";
import SchoolIcon from "@mui/icons-material/School";
import EventIcon from "@mui/icons-material/Event";
import AssignmentIcon from "@mui/icons-material/Assignment";
import DescriptionIcon from "@mui/icons-material/Description";
import PersonIcon from "@mui/icons-material/Person";
import { useNavigate } from "react-router-dom";

export default function AlunoHome() {
  const navigate = useNavigate();

  const cards = [
    {
      title: "Aulas e Atividades",
      description: "Visualizar aulas, conteúdos e atividades pendentes.",
      icon: <SchoolIcon fontSize="large" />,
      route: "/aluno/aulas"
    },
    {
      title: "Eventos e Calendário",
      description: "Consultar eventos acadêmicos e datas importantes.",
      icon: <EventIcon fontSize="large" />,
      route: "/aluno/eventos"
    },
    {
      title: "Boletim",
      description: "Visualizar notas, médias e desempenho.",
      icon: <AssignmentIcon fontSize="large" />,
      route: "/aluno/boletim"
    },
    {
      title: "Matrícula",
      description: "Consultar situação da matrícula e comprovantes.",
      icon: <DescriptionIcon fontSize="large" />,
      route: "/aluno/matricula"
    },
    {
      title: "Meu Perfil",
      description: "Visualizar seus dados e informações acadêmicas.",
      icon: <PersonIcon fontSize="large" />,
      route: "/aluno/perfil"
    }
  ];

  return (
    <AppLayout>
      <Typography variant="h4" gutterBottom>
        Painel do Aluno
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