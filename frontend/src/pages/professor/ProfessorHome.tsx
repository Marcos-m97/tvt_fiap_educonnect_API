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
import AssignmentIcon from "@mui/icons-material/Assignment";
import GradeIcon from "@mui/icons-material/Grade";
import EventIcon from "@mui/icons-material/Event";
import FactCheckIcon from "@mui/icons-material/FactCheck";
import { useNavigate } from "react-router-dom";

export default function ProfessorHome() {
  const navigate = useNavigate();

  const cards = [
    {
      title: "Criar Aulas",
      description: "Cadastrar novos conteúdos e materiais.",
      icon: <SchoolIcon fontSize="large" />,
      route: "/professor/aulas"
    },
    {
      title: "Criar Atividades",
      description: "Criar e gerenciar atividades para os alunos.",
      icon: <AssignmentIcon fontSize="large" />,
      route: "/professor/atividades"
    },
    {
      title: "Boletim",
      description: "Lançar e atualizar notas dos alunos.",
      icon: <GradeIcon fontSize="large" />,
      route: "/professor/boletim"
    },
    {
      title: "Eventos",
      description: "Criar eventos acadêmicos e comunicados.",
      icon: <EventIcon fontSize="large" />,
      route: "/professor/eventos"
    },
    {
      title: "Corrigir Atividades",
      description: "Avaliar e corrigir atividades enviadas.",
      icon: <FactCheckIcon fontSize="large" />,
      route: "/professor/correcoes"
    }
  ];

  return (
    <AppLayout>
      <Typography variant="h4" gutterBottom>
        Painel do Professor
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
