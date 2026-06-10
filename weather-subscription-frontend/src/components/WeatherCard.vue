<template>
  <div class="card weather-card">
    <h2>{{ weather.city }}, {{ weather.country }}</h2>
    <p class="description">{{ weather.description }}</p>

    <div class="grid">
      <div class="stat">
        <span class="stat-label">Temperature</span>
        <span class="stat-value">{{ weather.temperature.current }}°C</span>
        <span class="stat-sub">↓ {{ weather.temperature.min }}°C &nbsp; ↑ {{ weather.temperature.max }}°C</span>
      </div>

      <div class="stat">
        <span class="stat-label">Cloudiness</span>
        <span class="stat-value">{{ weather.cloudiness }}</span>
      </div>

      <div class="stat">
        <span class="stat-label">Humidity</span>
        <span class="stat-value">{{ weather.humidity }}%</span>
      </div>

      <div class="stat">
        <span class="stat-label">Pressure</span>
        <span class="stat-value">{{ weather.pressure }} hPa</span>
      </div>

      <div class="stat">
        <span class="stat-label">Wind Speed</span>
        <span class="stat-value">{{ weather.windSpeed }} m/s</span>
      </div>

      <div class="stat">
        <span class="stat-label">Sunrise</span>
        <span class="stat-value">{{ weather.sunrise }}</span>
      </div>

      <div class="stat">
        <span class="stat-label">Sunset</span>
        <span class="stat-value">{{ weather.sunset }}</span>
      </div>
    </div>
  </div>
</template>

<script setup>
defineProps({
  weather: {
    type: Object,
    required: true
  }
})
</script>

<style scoped>
.weather-card {
  background: var(--color-card);
  border-radius: 10px;
  padding: 2rem;
}

h2 {
  margin: 0 0 0.3rem;
  font-size: 1.4rem;
  color: var(--color-heading);
}

.description {
  margin: 0 0 1.5rem;
  color: var(--color-label);
  text-transform: capitalize;
  font-size: 0.95rem;
}

.grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(130px, 1fr));
  gap: 1rem;
}

.stat {
  display: flex;
  flex-direction: column;
  gap: 0.2rem;
  background: var(--color-stat-bg);
  border-radius: 8px;
  padding: 0.8rem;
}

.stat-label {
  font-size: 0.75rem;
  color: var(--color-label);
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.stat-value {
  font-size: 1.1rem;
  font-weight: 600;
  color: var(--color-text);
}

.stat-sub {
  font-size: 0.8rem;
  color: var(--color-label);
}
</style>
