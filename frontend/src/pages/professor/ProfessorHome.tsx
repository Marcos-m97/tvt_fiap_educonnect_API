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
import { useNavigate } from "react-router-dom";

export default function ProfessorHome() {
  const navigate = useNavigate();

  const cards = [
    {
      title: "Acadêmico",
      description:
        "Acesse suas turmas, gerencie aulas, atividades, alunos e notas.",
      icon: <SchoolIcon sx={{ fontSize: 40 }} />,
      route: "/professor/academico"
    },
    {
      title: "Eventos",
      description:
        "Criar e gerenciar eventos acadêmicos e comunicados.",
      icon: <EventIcon sx={{ fontSize: 40 }} />,
      route: "/professor/eventos"
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
              Professor
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
            Gerencie suas turmas, aulas, atividades e eventos acadêmicos.
          </Typography>
        </Box>

        {/* GRID PADRONIZADO */}
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
              sx={{
                p: 2,
                borderRadius: 3,
                border: "1px solid",
                borderColor: "divider",
                transition: "all 0.2s ease",
                "&:hover": {
                  boxShadow: 6,
                  transform: "translateY(-4px)"
                }
              }}
            >
              <CardContent>
                <Box display="flex" alignItems="center" gap={2} mb={2}>
                  {card.icon}
                  <Typography variant="h6" fontWeight={600}>
                    {card.title}
                  </Typography>
                </Box>

                <Typography variant="body2" color="text.secondary">
                  {card.description}
                </Typography>
              </CardContent>

              <CardActions sx={{ justifyContent: "flex-end", pr: 2 }}>
                <Button
                  variant="contained"
                  size="small"
                  onClick={() => navigate(card.route)}
                >
                  Acessar
                </Button>
              </CardActions>
            </Card>
          ))}
        </Box>

      </Box>
    </AppLayout>
  );
}