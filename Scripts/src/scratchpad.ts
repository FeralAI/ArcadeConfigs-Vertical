import mgapi from './modules/mobygames-api/index.js';

const data = await mgapi.getGamesRandom({ format: mgapi.MobyGamesFormat.full, limit: 100 });
console.log(data);
