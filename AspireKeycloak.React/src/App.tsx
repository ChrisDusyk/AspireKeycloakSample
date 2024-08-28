import { useEffect, useState } from "react";
import { hasAuthParams, useAuth } from "react-oidc-context";
import { useQuery } from "@tanstack/react-query";
import "./App.css";

function App() {
  const auth = useAuth();
  const [hasTriedSignin, setHasTriedSignin] = useState(false);

  useEffect(() => {
    if (
      !hasAuthParams() &&
      !auth.isAuthenticated &&
      !auth.isLoading &&
      !auth.activeNavigator &&
      !hasTriedSignin
    ) {
      auth.signinRedirect();
      setHasTriedSignin(true);
    }
  }, [auth, hasTriedSignin]);

  if (auth.error) {
    console.log("Auth error", auth.error);
    return <div>Error: {auth.error.message}</div>;
  }

  if (auth.isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <>
      {auth.isAuthenticated ? (
        <>
          <span>Hello {auth.user?.profile.name}</span>{" "}
          <button onClick={() => void auth.signoutRedirect()}>Sign out</button>
          <Weather />
        </>
      ) : (
        <button onClick={() => void auth.signinRedirect()}>Sign in</button>
      )}
    </>
  );
}

type WeatherForecast = {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
};

function Weather() {
  const auth = useAuth();

  const { data, isPending } = useQuery<WeatherForecast[]>({
    queryKey: ["weather"],
    queryFn: async () => {
      const token = auth.user?.access_token;
      const response = await fetch(
        `${import.meta.env.VITE_API_URL}/weatherforecast`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      return await response.json();
    },
  });

  if (isPending) {
    return <div>Loading...</div>;
  }

  return (
    <table>
      <thead>
        <tr>
          <th>Date</th>
          <th>Temperature (C)</th>
          <th>Temperature (F)</th>
          <th>Summary</th>
        </tr>
      </thead>
      <tbody>
        {data!.map((forecast, index) => (
          <tr key={index}>
            <td>{forecast.date.toString()}</td>
            <td>{forecast.temperatureC}</td>
            <td>{forecast.temperatureF}</td>
            <td>{forecast.summary}</td>
          </tr>
        ))}
      </tbody>
    </table>
  );
}

export default App;
